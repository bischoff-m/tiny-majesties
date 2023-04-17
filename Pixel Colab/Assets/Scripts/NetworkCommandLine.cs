using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkCommandLine : MonoBehaviour
{
    private NetworkManager _netManager;

    private void Start()
    {
        _netManager = GetComponentInParent<NetworkManager>();

        if (Application.isEditor)
        {
            _netManager.StartHost();
        }
        else
        {
            _netManager.StartClient();
        }

        return;

        try
        {
            var args = GetCommandlineArgs();
            Debug.Log("Did work????????????????");
            Debug.Log(args);

            if (args.TryGetValue("-mode", out var mode))
            {
                switch (mode)
                {
                    case "server":
                        _netManager.StartServer();
                        break;
                    case "host":
                        _netManager.StartHost();
                        break;
                    case "client":
                        _netManager.StartClient();
                        break;
                }
            }
            else
            {
                _netManager.StartServer();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Debug.Log("Did not work................");
            _netManager.StartServer();
        }
    }

    private Dictionary<string, string> GetCommandlineArgs()
    {
        var argDictionary = new Dictionary<string, string>();

        var args = System.Environment.GetCommandLineArgs();

        for (var i = 0; i < args.Length; ++i)
        {
            var arg = args[i].ToLower();
            if (arg.StartsWith("-"))
            {
                var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                value = (value?.StartsWith("-") ?? false) ? null : value;

                argDictionary.Add(arg, value);
            }
        }
        return argDictionary;
    }
}