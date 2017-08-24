using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

public class StatistiqueManager : MonoBehaviour
{

    private readonly string statisticsRootFolderName = "Evaluation - " + DateTime.Now.ToString("yyyy_MM_dd");
    private readonly string statFileName = "Simulation_" + DateTime.Now.ToString("yyyy_MM_dd_HH-mm-ss") + ".csv";
    private string pathString;
    private const string separator = ";";

    public static StatistiqueManager Instance
    {
        get;
        private set;
    }

    #region Constructors
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple StatistiqueController in the Scene.");
            return;
        }
        Instance = this;
    }

    #endregion

    public void InitStatFile()
    {
        StringBuilder builder = new StringBuilder();
        if (!Directory.Exists(statisticsRootFolderName))
        {
            Directory.CreateDirectory(statisticsRootFolderName);
        }
        pathString = System.IO.Path.Combine(statisticsRootFolderName, statFileName);
        WriteHeadFile(builder);
        WriteHeadTable(builder);
        WriteToFile(pathString, builder.ToString());
    }

    public void SaveGenotype(Genotype genotype, uint genotypesSaved)
    {
        string saveFolder = statisticsRootFolderName + "/";


        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        genotype.SaveToFile(saveFolder + "Genotype - Finished as " + genotypesSaved + ".txt");

    }


    public void AppendDataToStat(String data)
    {
        if (String.IsNullOrEmpty(data)) throw new ArgumentNullException(data);
        if (!File.Exists(pathString)) throw new FileNotFoundException("The file "+pathString+" has not been initialized");
        System.IO.StreamWriter file = new System.IO.StreamWriter(pathString, true);
        file.WriteLine(data);
        file.Close();

    }

    private void WriteHeadFile(StringBuilder builder)
    {
        builder.AppendLine("PARAMETRES GENERAUX DE LA SIMULATION");
        builder.AppendLine("Nombre d'individu par génération"+separator + GameStateManager.Instance.appSettings.populationSize);
        builder.AppendLine("Nombre de drones sélectionnés pour reproduction" + separator + GameStateManager.Instance.appSettings.nbSelectedDrone);
        builder.AppendLine("Nom du circuit" + separator + GameStateManager.Instance.appSettings.trackId);
        builder.AppendLine("Pourcentage de la population mutée" + separator + GameStateManager.Instance.appSettings.defMutationPerc);
        builder.AppendLine("Probabilité d'un croisement de gène lors de la mutation" + separator + GameStateManager.Instance.appSettings.defCrossSwapProb);
        builder.AppendLine("Probabilité de mutation" + separator + GameStateManager.Instance.appSettings.defMutationProb);
        builder.AppendLine("");
    }

    private void WriteHeadTable(StringBuilder builder)
    {
        builder.AppendLine("Numéro de génération" + separator+"Evaluation meilleur Drone" + separator+"Evaluation moyenne Drone");
    }

    private void WriteToFile(string pathToFile, String data)
    {
        if (String.IsNullOrEmpty(pathToFile)) throw new ArgumentNullException(pathToFile);
        if (String.IsNullOrEmpty(data)) throw new ArgumentNullException(data);
        File.AppendAllText(pathToFile, data);

    }
}
