using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.CommandLineUtils;
using System.IO;

namespace Das.WorkItemSigner
{
    class Program
    {
        static int Main(string[] args)
        {
            var cla = new CommandLineApplication(throwOnUnexpectedArg: true)
            {
                FullName = "Autodesk Forge Design Automation Service - WorkItem signer",
                Description = "Manages digital signatures for workitems that are submitted with 3-legged oauth tokens."
            };
            cla.HelpOption("-? | -h | --help");
            var version = "1.0";
            cla.VersionOption("--version", version, string.Format("version {0}", version));
            cla.Command("generate", (t) =>
            {
                var keyFile = t.Argument("keyFile", "Name of the file that will be written with the secret. You must safeguard it accordingly.");
                t.Description = "Generate secret in a json file.";
                t.HelpOption("-? | -h | --help");
                t.OnExecute(() => { return OnGenerate(keyFile); });
            });
            cla.Command("sign", (t) =>
            {
                var keyFile = t.Argument("keyFile", "Name of the file containing the secret.");
                var input = t.Argument("input", "The input string that will be signed.");
                t.Description = "Sign a string. The signature, base64 encoded, will be sent to standard output.";
                t.HelpOption("-? | -h | --help");
                t.OnExecute(() => { return OnSign(keyFile, input); });
            });
            cla.Command("export", (t) =>
            {
                var keyFile = t.Argument("keyFile", "Name of the file containing the secret.");
                var input = t.Argument("outputFile", "Name of the file that will be written with the public key.");
                t.Description = "Export the public key from the secrete file.";
                t.HelpOption("-? | -h | --help");
                t.OnExecute(() => { return OnExport(keyFile, input); });
            });
            if (args.Length == 0)
                cla.ShowHelp();
            try
            {
                return cla.Execute(args);
            }
            catch (CommandParsingException)
            {
                return -1;
            }
        }
        static int OnGenerate(CommandArgument keyFile)
        {
            var signer = Signer.Create();
            signer.Save(keyFile.Value, includePrivateParameters: true);
            return 0;
        }

        static int OnSign(CommandArgument keyFile, CommandArgument input)
        {
            var signer = Signer.Load(keyFile.Value);
            var signed = signer.Sign(input.Value);
            Console.WriteLine(signed);
            return 0;
        }
        static int OnExport(CommandArgument keyFile, CommandArgument outputFile)
        {
            var signer = Signer.Load(keyFile.Value);
            signer.Save(outputFile.Value, includePrivateParameters: false);
            return 0;
        }
    }
    public class Signer
    {
        private RSA rsa;
        private Signer(RSA r)
        {
            rsa = r;
        }
        public static Signer Load(string keyFile)
        {
            return FromJson(File.ReadAllText(keyFile));
        }
        public static Signer FromJson(string json)
        {
            var rsp = RSA.Create();
            var prams = JsonConvert.DeserializeObject<RSAParameters>(json);
            rsp.ImportParameters(prams);
            return new Signer(rsp);
        }
        public static Signer Create()
        {
            return new Signer(RSA.Create());
        }

        public void Save(string keyFile, bool includePrivateParameters)
        {
            File.WriteAllText(keyFile, ToJson(includePrivateParameters));
        }
        public string ToJson(bool includePrivateParameters)
        {
            var prams = rsa.ExportParameters(includePrivateParameters);
            return JsonConvert.SerializeObject(prams, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }
        public string Sign(string input)
        {
            var bytes = rsa.SignData(Encoding.Unicode.GetBytes(input), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(bytes);
        }
    }
}