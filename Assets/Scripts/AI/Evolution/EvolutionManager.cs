/// Author: Samuel Arzt
/// Date: March 2017


#region Includes
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
#endregion

/// <summary>
/// Singleton class for managing the evolutionary processes.
/// </summary>
/// 
namespace Drone.AI.Evolution
{
    public class EvolutionManager : MonoBehaviour
    {
        #region Members
        private static System.Random randomizer = new System.Random();

        public static EvolutionManager Instance
        {
            get;
            private set;
        }

        public string PathToInitGenotypeFrom
        {
            get;
            set;
        }

        // How many of the first to finish the course should be saved to file, to be set in Unity Editor
        [SerializeField]
        private uint SaveFirstNGenotype = 0;
        private uint genotypesSaved = 0;

        // Population size, to be set in Unity Editor
        private int PopulationSize;

        // Whether to use elitist selection or remainder stochastic sampling, to be set in Unity Editor
        [SerializeField]
        private bool ElitistSelection = true;

        // Topology of the agent's FNN, to be set in Unity Editor
        [SerializeField]
        private uint[] FNNTopology;

        // The current population of agents.
        private List<Agent> agents = new List<Agent>();

        private GeneticAlgorithm geneticAlgorithm;

        public Boolean CanSaveBestGenotype
        {
            get;
            set;
        }

        /// <summary>
        /// The age of the current generation.
        /// </summary>
        public uint GenerationCount
        {
            get { return geneticAlgorithm.GenerationCount; }
        }
        #endregion

        #region Constructors
        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("More than one EvolutionManager in the Scene.");
                return;
            }
            Instance = this;
            PopulationSize = GameStateManager.Instance.appSettings.populationSize;
        }
        #endregion

        #region Methods


        public void LaunchActivity()
        {
            StatistiqueManager.Instance.InitStatFile();
            StartEvolution();
        }

        public void PauseResumeActivity()
        {
            TrackManager.Instance.PauseResumeSimulation();
        }


        /// <summary>
        /// Starts the evolutionary process.
        /// </summary>
        public void StartEvolution()
        {
            //Create neural network to determine parameter count
            NeuralNetwork nn = new NeuralNetwork(FNNTopology);

            TrackManager.Instance.ClearDrones();

            //Setup genetic algorithm
            geneticAlgorithm = new GeneticAlgorithm((uint)nn.WeightCount, (uint)PopulationSize, PathToInitGenotypeFrom);
            genotypesSaved = 0;

            geneticAlgorithm.Evaluation = StartEvaluation;
            if (PathToInitGenotypeFrom != null)
            {
                geneticAlgorithm.InitialisePopulation = geneticAlgorithm.InitializePopulationWithGenotype;
                PathToInitGenotypeFrom = null;
            }

            if (ElitistSelection)
            {
                //Second configuration
                geneticAlgorithm.Selection = GeneticAlgorithm.DefaultSelectionOperator;
                geneticAlgorithm.Recombination = RandomRecombination;
                geneticAlgorithm.Mutation = MutateAllButBestTwo;
            }
            else
            {
                //First configuration
                geneticAlgorithm.Selection = RemainderStochasticSampling;
                geneticAlgorithm.Recombination = RandomRecombination;
                geneticAlgorithm.Mutation = MutateAllButBestTwo;
            }

            TrackManager.Instance.AllAgentsDied += geneticAlgorithm.EvaluationFinished;

            geneticAlgorithm.FitnessCalculationFinished += CheckForTrackFinished;

            geneticAlgorithm.Start();
        }

        // Checks the current population and saves genotypes to a file if their evaluation is greater than or equal to 1
        private void CheckForTrackFinished(IEnumerable<Genotype> currentPopulation)
        {
            if (!CanSaveBestGenotype) return;

            foreach (Genotype genotype in currentPopulation)
            {
                if (genotype.Evaluation >= 0.20 && genotype.Evaluation == geneticAlgorithm.bestReward)
                {
                    StatistiqueManager.Instance.SaveGenotype(genotype, (++genotypesSaved));
                }
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(GenerationCount + ";" + geneticAlgorithm.bestReward + ";" + GeneticAlgorithm.averageEvaluation);
            StatistiqueManager.Instance.AppendDataToStat(builder.ToString());

        }

        // Starts the evaluation by first creating new agents from the current population and then restarting the track manager.
        private void StartEvaluation(IEnumerable<Genotype> currentPopulation)
        {
            //Create new agents from currentPopulation
            agents.Clear();

            //Agents Initialization 
            foreach (Genotype genotype in currentPopulation)
                agents.Add(new Agent(genotype, MathHelper.SigmoidFunction, FNNTopology));

            TrackManager.Instance.ClearDrones();

            TrackManager.Instance.SetAgentList(agents);

            TrackManager.Instance.StartRace();
        }

        private float ReLUFunction(float x)
        {
            return Math.Max(0, x);
        }


        #region GA Operators
        // Selection operator for the genetic algorithm, using a method called remainder stochastic sampling.
        private List<Genotype> RemainderStochasticSampling(List<Genotype> currentPopulation)
        {
            List<Genotype> intermediatePopulation = new List<Genotype>();
            //Put integer portion of genotypes into intermediatePopulation
            //Assumes that currentPopulation is already sorted
            foreach (Genotype genotype in currentPopulation)
            {
                if (genotype.Fitness < 1)
                    break;
                else
                {
                    for (int i = 0; i < (int)genotype.Fitness; i++)
                        intermediatePopulation.Add(new Genotype(genotype.GetParameterCopy()));
                }
            }

            //Put remainder portion of genotypes into intermediatePopulation
            foreach (Genotype genotype in currentPopulation)
            {
                float remainder = genotype.Fitness - (int)genotype.Fitness;
                if (randomizer.NextDouble() < remainder)
                    intermediatePopulation.Add(new Genotype(genotype.GetParameterCopy()));
            }

            return intermediatePopulation;
        }

        // Recombination operator for the genetic algorithm, recombining random genotypes of the intermediate population
        private List<Genotype> RandomRecombination(List<Genotype> intermediatePopulation, uint newPopulationSize)
        {
            //Check arguments
            if (intermediatePopulation.Count < 2)
                throw new System.ArgumentException("The intermediate population has to be at least of size 2 for this operator.");

            List<Genotype> newPopulation = new List<Genotype>();
            //Always add best two (unmodified)
            newPopulation.Add(intermediatePopulation[0]);
            newPopulation.Add(intermediatePopulation[1]);


            while (newPopulation.Count < newPopulationSize)
            {
                //Get two random indices that are not the same
                int randomIndex1 = randomizer.Next(0, intermediatePopulation.Count), randomIndex2;
                do
                {
                    randomIndex2 = randomizer.Next(0, intermediatePopulation.Count);
                } while (randomIndex2 == randomIndex1);

                Genotype offspring1, offspring2;
                GeneticAlgorithm.CompleteCrossover(intermediatePopulation[randomIndex1], intermediatePopulation[randomIndex2],
                    GameStateManager.Instance.appSettings.defCrossSwapProb, out offspring1, out offspring2);

                newPopulation.Add(offspring1);
                if (newPopulation.Count < newPopulationSize)
                    newPopulation.Add(offspring2);
            }

            return newPopulation;
        }

        // Mutates all members of the new population with the default probability, while leaving the first 2 genotypes in the list untouched.
        private void MutateAllButBestTwo(List<Genotype> newPopulation)
        {
            for (int i = 2; i < newPopulation.Count; i++)
            {
                if (randomizer.NextDouble() < GameStateManager.Instance.appSettings.defMutationPerc)
                    GeneticAlgorithm.MutateGenotype(newPopulation[i], GameStateManager.Instance.appSettings.defMutationProb, GameStateManager.Instance.appSettings.defMutationAmount);
            }
        }

        // Mutates all members of the new population with the default parameters
        private void MutateAll(List<Genotype> newPopulation)
        {
            foreach (Genotype genotype in newPopulation)
            {
                if (randomizer.NextDouble() < GameStateManager.Instance.appSettings.defMutationPerc)
                    GeneticAlgorithm.MutateGenotype(genotype, GameStateManager.Instance.appSettings.defMutationProb, GameStateManager.Instance.appSettings.defMutationAmount);
            }
        }
        #endregion
        #endregion

    }
}
