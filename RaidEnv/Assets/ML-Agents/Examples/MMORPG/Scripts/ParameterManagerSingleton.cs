using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterManagerSingleton
{

    public static ParameterManagerSingleton Instance;
    private Hashtable ParsedArgs;


    public static ParameterManagerSingleton GetInstance()
    {
        if (Instance == null)
        {
            Instance = new ParameterManagerSingleton();
            Instance.Initialize();
        }
        return Instance;
    }

    // Start is called before the first frame update
    void Initialize()
    {
        ParsedArgs = new Hashtable();
        ParseCommandlineArgs();
        RegisterDefaultValues();

        // Make Log folder if not exists
        if (!System.IO.Directory.Exists(ParsedArgs["logPath"].ToString())) {
            System.IO.Directory.CreateDirectory(ParsedArgs["logPath"].ToString());
        }
        
        Debug.Log(this);
    }

    private void RegisterDefaultValues() 
    {
        // Check key existance in ParsedArgs
        // If not, add default value
        if (!ParsedArgs.ContainsKey("runId")) { ParsedArgs.Add("runId", "default-id"); }
        if (!ParsedArgs.ContainsKey("logPath")) { ParsedArgs.Add("logPath", Application.dataPath + "/Log/"); }
        if (!ParsedArgs.ContainsKey("pcgHeuristic")) { ParsedArgs.Add("pcgHeuristic", false); }
        if (!ParsedArgs.ContainsKey("pcgRandom")) { ParsedArgs.Add("pcgRandom", false); }
        if (!ParsedArgs.ContainsKey("pcgSaveEpisodeLimit")) { ParsedArgs.Add("pcgSaveEpisodeLimit", 0); }
    }
    
    private void ParseCommandlineArgs()
    {
        // Parse command line arguments and save it on a hashtable
        string[] args = System.Environment.GetCommandLineArgs();


        int idx = 0;
        while (idx < args.Length)
        {
            if (args[idx].Contains("--runId")) 
            {
                ParsedArgs.Add("runId", args[++idx]);
            }
            else if (args[idx].Contains("--logPath")) 
            {
                ParsedArgs.Add("logPath", args[++idx]);
            }
            else if (args[idx].Contains("--pcgSaveCreatedSkill")) 
            {
                ParsedArgs.Add("pcgSaveCreatedSkill", true);
            }
            else if (args[idx].Contains("--pcgHeuristic")) 
            {
                ParsedArgs.Add("pcgHeuristic", true);
            }
            else if (args[idx].Contains("--pcgRandom")) 
            {
                ParsedArgs.Add("pcgRandom", true);
            }
            else if (args[idx].Contains("--pcgSaveEpisodeLimit")) 
            {
                ParsedArgs.Add("pcgSaveEpisodeLimit", args[++idx]);
            }
            else if (args[idx].Contains("--pcgSimulationLimit")) 
            {
                ParsedArgs.Add("pcgSimulationLimit", args[++idx]);
            }
            else if (args[idx].Contains("--pcgStrictEpisodeLength")) 
            {
                ParsedArgs.Add("pcgStrictEpisodeLength", true);
            }
            else if (args[idx].Contains("--skillPath"))
            {
                ParsedArgs.Add("skillPath", args[++idx]);
            }
            else if (args[idx].Contains("--maEvalEpisodeLimit"))
            {
                ParsedArgs.Add("maEvalEpisodeLimit", args[++idx]);
            }
            else if (args[idx].Contains("--healthCheck"))
            {
                ParsedArgs.Add("healthCheck", true);
            }
            idx++;
        }
   }

    public override string ToString() 
    {
        string result = "[ParameterManagerSingleton]\n";
        foreach (DictionaryEntry entry in ParsedArgs)
        {
            result += entry.Key + " : " + entry.Value + "\n";
        }
        return result;
    }

    #nullable enable
    public object? GetParam(string key) 
    {
        if(ParsedArgs.ContainsKey(key)) {
            return ParsedArgs[key].ToString();
        } else {
            return null;
        }
    }
    #nullable disable
    
    public bool HasParam(string key) 
    {
        return ParsedArgs.ContainsKey(key);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
