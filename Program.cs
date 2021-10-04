using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace generator {

    class Program {
        static String[] _dictionary;
        static String[] _country;
        static String[] _names;
        static String[] _lastnames;
        static String[] _declaredTags;
        static String[] _releaseReason;
        static String[] _subjectTerminology;
        static String[] _caseNumbers;
        static String[] _text;
        static String[] _materials = new String[] { "Article", "Book", "Paper", "Presentation" };
        static String[] _fundingSources = new string[] {
            "CAASD Non-Direct Project",
            "CAMH FFRDC Contracts",
            "DHS FFRDC Contracts",
            "DoD FFRDC Contracts",
            "FAA FFRDC Contracts",
            "FAA MOIE",
            "Independent Effort",
            "International Contracts",
            "IRS and VA FFRDC Contracts",
            "MSR",
            "NIST FFRDC Contracts",
            "Non-Sponsored",
            "NSEC MOIE",
            "Other Contracts"
        };
        static String[] _level1 = new string[] {
            "Corporate Ops & Transformation",
            "HR, Strat Comm & BD Ops/Dev",
            "MITRE Legacy",
            "MITRE National Security Sector",
            "MITRE Public Sector",
            "Programs & Technology",

        };
        static String[] _status = new string[] { "Active", "Cancelled", "Closed" };

        static int _genFiles = 70;
        static void Main (string[] args) {
            BuildDictionary ();
            GenerateAllFiles ();
        }

        public static void CreateText () {
            var newFileText = new List<string> ();
            foreach (string line in File.ReadLines ("./text.txt")) {
                var lines = CleanText (line, 1500).Split ("\n");
                foreach (var lin in lines) {
                    newFileText.Add (lin);
                }
            }
            // Console.WriteLine (newFileText.Count ());
            TextWriter tw = new StreamWriter ("texttest.txt");
            foreach (String s in newFileText) {
                tw.WriteLine (s);
            }
            tw.Close ();
        }
        public static string CleanText (String line, int splitAt) {
            // List<String> lines = new List<String> ();

            return string.Join (Environment.NewLine, line.Split ().ToList ()
                .Select ((word, index) => new { word, index })
                .GroupBy (x => x.index / splitAt)
                .Select (grp => string.Join (" ", grp.Select (x => x.word))));
        }

        public static void GenerateAllFiles () {
            var output = "./output/";

            if (!Directory.Exists (output)) {
                Directory.CreateDirectory (output);
            }

            var watch = System.Diagnostics.Stopwatch.StartNew ();
            var elapsedMs = watch.ElapsedMilliseconds;

            for (int i = 0; i < _genFiles; i++) {
                var data = GenerateMPL ();
                CreateFile (Path.Combine (output, $"mpl_{data.sdl_id}.json"), JsonConvert.SerializeObject (data, Formatting.Indented));
            }
            for (int i = 0; i < _genFiles; i++) {
                var data = GeneratePWS ();
                CreateFile (Path.Combine (output, $"pws_{data.sdl_id}.json"), JsonConvert.SerializeObject (data, Formatting.Indented));
            }
            for (int i = 0; i < _genFiles; i++) {
                var data = GeneratePlatform ();
                CreateFile (Path.Combine (output, $"plat_{data.sdl_id}.json"), JsonConvert.SerializeObject (data, Formatting.Indented));
            }
            for (int i = 0; i < _genFiles; i++) {
                var data = GeneratePRC ();
                CreateFile (Path.Combine (output, $"prc_{data.sdl_id}.json"), JsonConvert.SerializeObject (data, Formatting.Indented));
            }
            for (int i = 0; i < _genFiles; i++) {
                var data = GenerateMIP ();
                CreateFile (Path.Combine (output, $"mip_{data.sdl_id}.json"), JsonConvert.SerializeObject (data, Formatting.Indented));
            }
            for (int i = 0; i < _genFiles; i++) {
                var data = GenerateMVC ();
                CreateFile (Path.Combine (output, $"mvc_{data.sdl_id}.json"), JsonConvert.SerializeObject (data, Formatting.Indented));
            }
            for (int i = 0; i < _genFiles; i++) {
                var data = Generatetcas ();
                CreateFile (Path.Combine (output, $"tca_{data.sdl_id}.json"), JsonConvert.SerializeObject (data, Formatting.Indented));
            }

            watch.Stop ();
            Console.WriteLine ("realtime: {0} ms", watch.ElapsedMilliseconds);
            // Console.WriteLine (Newtonsoft.Json.JsonConvert.SerializeObject (mpl));
        }
        public static void CreateFile (string filePath, string json) {
            using (FileStream fs = File.Create (filePath)) {
                byte[] info = new UTF8Encoding (true).GetBytes (json);
                fs.Write (info, 0, info.Length);
            }
        }

        public static MPL GenerateMPL () {
            var insight = JsonConvert.SerializeObject (GenerateInsight ());
            MPL mpl = JsonConvert.DeserializeObject<MPL> (insight);
            var creators = GetNumber (1, 4);
            mpl.productName = $"{GetWords(2)}";
            mpl.uploadedate = GetDate (2010, 2020);
            mpl.productUrl = $"http://{GetWords(1)}.com";
            mpl.creatorNames = "";
            for (int i = 0; i <= creators; i++) {
                var creator = $"{GetName()} {GetChar(1)} {GetLastName()}";
                mpl.creatorNames += creator;
                mpl.creatorNames += i != creators ? ";" : "";
            }
            mpl.uploaded = GetDate (2010, 2020);
            mpl.sdl_extracted_summary = GetText ();
            return mpl;
        }
        public static PWS GeneratePWS () {
            var insight = JsonConvert.SerializeObject (GenerateInsight ());
            PWS pws = JsonConvert.DeserializeObject<PWS> (insight);

            pws.source_library = $"SRC-{GetWords(1)}";
            pws.file_name = $"{GetWords(1)}.ext";
            pws.document_url = $"http://{GetWords(1)}{GetWords(1)}.com";
            pws.uploaded_by = $"{GetName()} {GetChar(1)} {GetLastName()}";
            pws.last_modified = GetDate(2000,2020).ToShortDateString();
            return pws;
        }
        public static Platform GeneratePlatform () {
            var numNames = GetNumber (3, 10);
            var numPractices = GetNumber (3, 10);
            var numPractice = GetNumber (1, 4);
            var insight = JsonConvert.SerializeObject (GenerateInsight ());
            Platform data = JsonConvert.DeserializeObject<Platform> (insight);
            data.changed = GetDate (2019, 2024);
            data.field_launch_date = GetDate (2019, 2024);

            data.field_platform_contacts = "";
            for (int i = 0; i <= numNames; i++) {
                var name = GetFullName ();
                data.field_platform_contacts += i == 0 ? name : "| " + name;
            }
            data.field_communities_of_practice = "";
            for (int i = 0; i <= numPractices; i++) {
                var name = GetWords (numPractice);
                data.field_communities_of_practice += i == 0 ? name : "| " + name;
            }
            data.platform_leader_name = GetFullName ();
            data.field_banner_subhead = GetWords (2);
            data.platform_url = $"https://{GetWords(1)}.com";

            return data;
        }
        public static PRC GeneratePRC () {
            var insight = JsonConvert.SerializeObject (GenerateInsight ());
            PRC data = JsonConvert.DeserializeObject<PRC> (insight);

            return data;
        }
        public static MIP GenerateMIP () {
            var insight = JsonConvert.SerializeObject (GenerateInsight ());
            MIP data = JsonConvert.DeserializeObject<MIP> (insight);
            var numPorts = GetNumber (1, 5);
            data.chargeCode = $"{GetChar(3)}";
            data.longName = $"{GetChar(10)}-{GetNumber(1000,9999)}";
            data.endDate = GetDate (2000, 2020);
            data.phonebookDisplayName = $"{GetName()} {GetLastName()}";
            data.startDate = GetDate (2000, 2020);
            data.status = GetStatus ();
            data.name = GetWords (2);

            data.portfolios = "";
            for (int i = 0; i <= numPorts; i++) {
                var portf = $"{GetWords(1)}";
                data.portfolios += i != numPorts ? $"{portf}|" : $"{portf}";
            }
            data.project_url = $"http://{GetWords(1)}.com";
            return data;
        }
        public static MVC GenerateMVC () {
            var insight = JsonConvert.SerializeObject (GenerateInsight ());
            MVC data = JsonConvert.DeserializeObject<MVC> (insight);

            data.project_name = $"{GetChar(3)}";
            data.project_sponsor = $"{GetChar(10)}-{GetNumber(1000,9999)}";
            data.project_end = GetDate (2000, 2020);

            data.portfolio = GetWords (2);
            data.super_portfolio = GetChar (3).ToUpper ();
            data.sub_portfolio = $"{GetChar(3)} - {GetWords(2)}";
            data.clarify = GetStatus ();
            data.project_url = $"http://{GetWords(1)}.com";
            data.project_page_charge_code = $"{GetNumber(100,999)}{GetChar(4).ToUpper()}";
            data.project_leader = GetFullName();
            return data;
        }
        public static tcas Generatetcas () {
            var insight = JsonConvert.SerializeObject (GenerateInsight ());
            tcas data = JsonConvert.DeserializeObject<tcas> (insight);

            data.field_tca_organizationleadername = $"{GetName()} {GetChar(1).ToUpper()} {GetLastName()}";
            data.changed = GetDate (2000, 2020);
            data.field_tca_short_name = GetWords (3);
            data.capability_rul = $"{GetWords(1)}.com";
            return data;
        }
        public static Insight GenerateInsight () {
            var rnd = new Random ();
            int titleRot = rnd.Next (1, 5);
            int ranTag = rnd.Next (3, 6);
            int ranCases = rnd.Next (1, 5);
            int ranFunding = rnd.Next (0, _fundingSources.Length - 1);
            return new Insight {
                sdl_date = GetDate (2020, 2020),
                    countryPublished = GetCountry (),
                    conference = $"{GetWords(2)} {GetChar(2)} {GetName()}",
                    originalAuthorName = $"{GetName()} {GetChar(1)} {GetLastName()}",
                    title = $"{GetWords(titleRot)}",
                    declaredTags = $"{GetTags(ranTag)}",
                    releaseReason = $"{GetWords(1)}/{GetWords(1)}",
                    docName = $"{GetChar(2).ToUpper()}_{GetNumber(10,99)}_{GetNumber(1000,9999)}",
                    fundingCenter = GetNumber (10, 100),
                    resourceURL = $"https://{GetWords(1)}.com",
                    fundingDepartment = $"{GetChar(2)}{GetNumber(10,99)}",
                    caseNumber = $"{GetNumber(10,99)}-{GetNumber(1000,9999)}",
                    publicationDate = $"{GetDate(2017,2020)}",
                    releaseYear = GetNumber (2000, 2020),
                    releaseStatement = $"{GetReleaseReason()}",
                    approver = $"${GetName()} ${GetLastName()}",
                    handCarry = GetNumber (0, 10),
                    authorDivision = $"{GetChar(2)}{GetNumber(10,99)}",
                    copyrightOwner = $"{GetName()} {GetLastName()}",
                    lastModifiedDate = $"{GetDate(2000,2020)}",
                    releaseDate = $"{GetDate(2000,2020)}",
                    onMitrePublicSrvr = GetNumber (0, 1),
                    projectNumber = $"{GetNumber(1000,9999)}{GetChar(4).ToUpper()}{GetNumber(10,99)}",
                    materialType = $"{GetMaterial()}",
                    publicationType = $"{GetMaterial()}",
                    authorCenter = GetNumber (10, 99),
                    originalAuthorID = $"{GetName()}",
                    mitrePublicServer = GetNumber (0, 1),
                    subjectTerminology = GetSubjectTerminology (),
                    dateEntered = GetDate (2000, 2015).ToString (),
                    documentInfoURL = $"https://{GetWords(5)}.com",
                    softShell = GetNumber (0, 1),
                    publishedOnNonMITREServer = GetNumber (0, 1),
                    priorCaseNumbers = GetCaseNumbers (ranCases),
                    organization = $"{GetChar(2)}{GetNumber(10,99)}",
                    authorDepartment = $"{GetChar(2)}{GetNumber(10,99)}",
                    publicationYear = GetNumber (1990, 2020),
                    sensitivity = "Public",
                    copyrightText = "(c) 2016 The MITRE Corporation All Rights Reserved",
                    fundingSource = _fundingSources[ranFunding],
                    level1 = _level1[rnd.Next (0, _level1.Length)],
                    fundingDivision = $"{GetWords(5)}",
                    publishedOutsideUSA = GetNumber (0, 1),
                    level3 = $"{GetChar(2)}{GetNumber(10,99)}",
                    level2 = $"{GetChar(2)}{GetNumber(10,99)}",
                    sdl_id = $"{GetGuid()}",
                    text = $"{GetText()}",
                    updated_at = $"{GetDate(1990, 2020)}",
                    created_at = $"{GetDate(1990, 2020)}"
            };
        }

        private static String GetFullName () {
            return $"{GetName()} {GetChar(1).ToUpper()} {GetLastName()}";
        }
        private static String GetName () {
            var r = new Random ();
            var randomLineNumber = r.Next (0, _names.Length - 1);
            return _names[randomLineNumber];
        }
        private static String GetLastName () {
            var r = new Random ();
            var randomLineNumber = r.Next (0, _lastnames.Length - 1);
            return _lastnames[randomLineNumber];
        }
        private static DateTime GetDate (int startYear, int endYear) {
            var gen = new Random ();
            var start = new DateTime (startYear, 1, 1);
            var end = (new DateTime (endYear, 12, 31));
            int range = (end - start).Days;
            return start.AddDays (gen.Next (range));
        }
        private static String GetWords (int numberOfWords) {
            var line = "";
            for (int i = 0; i < numberOfWords; i++) {
                var r = new Random ();
                var randomLineNumber = r.Next (0, _dictionary.Length - 1);
                line += _dictionary[randomLineNumber] + " ";
            }
            return line.TrimEnd ();
        }
        private static String GetCountry () {
            var r = new Random ();
            var randomLineNumber = r.Next (0, _country.Length - 1);
            return _country[randomLineNumber];
        }
        private static String GetText () {
            var r = new Random ();
            var randomLineNumber = r.Next (0, _text.Length - 1);
            return _text[randomLineNumber];
        }
        private static String GetSubjectTerminology () {
            var r = new Random ();
            var randomLineNumber = r.Next (0, _subjectTerminology.Length - 1);
            return _subjectTerminology[randomLineNumber];
        }
        private static String GetChar (int numberOfChars) {
            var text = "";
            for (int i = 0; i < numberOfChars; i++) {
                Random rnd = new Random ();
                var randomChar = (char) rnd.Next ('a', 'z');
                text += randomChar;
            }
            return text;
        }
        private static int GetNumber (int start, int end) {
            var rnd = new Random ();
            return rnd.Next (start, end);
        }
        private static String GetTags (int numberOfTags) {
            var r = new Random ();
            var line = "";
            for (int i = 0; i < numberOfTags; i++) {
                var randomLineNumber = r.Next (0, _declaredTags.Length - 1);
                line += _declaredTags[randomLineNumber];
                line += i < numberOfTags - 1 ? "|" : "";
            }
            return line;
        }
        private static string GetStatus () {
            var rnd = new Random ();
            return _status[rnd.Next (0, _status.Length - 1)];
        }
        private static String GetReleaseReason () {
            var r = new Random ();
            var randomLineNumber = r.Next (0, _releaseReason.Length - 1);
            return _releaseReason[randomLineNumber];
        }
        private static String GetMaterial () {
            var rnd = new Random ();
            return _materials[rnd.Next (0, _materials.Length - 1)];
        }
        private static String GetCaseNumbers (int numberOfTags) {
            var r = new Random ();
            var line = "";
            for (int i = 0; i < numberOfTags; i++) {
                var randomLineNumber = r.Next (0, _caseNumbers.Length - 1);
                line += _caseNumbers[randomLineNumber];
                line += i < numberOfTags - 1 ? "|" : "";
            }
            return line;
        }
        private static String GetGuid () {
            var guid = Guid.NewGuid ().ToString ();
            return guid.Replace ("-", "");
        }
        private static void BuildDictionary () {
            _dictionary = File.ReadAllLines ("/usr/share/dict/words");
            _country = File.ReadAllLines ("./countries.txt");
            _names = File.ReadAllLines ("./names.txt");
            _lastnames = File.ReadAllLines ("./lastnames.txt");
            _declaredTags = File.ReadAllLines ("./declaredTags.txt");
            _releaseReason = File.ReadAllLines ("./releaseReasons.txt");
            _subjectTerminology = File.ReadAllLines ("./subjectTerminologies.txt");
            _caseNumbers = File.ReadAllLines ("./caseNumbers.txt");
            _text = File.ReadAllLines ("./texttest.txt");
        }

    }

}

public class MPL : Insight {
    public string sdl_source_type = "MPL";
    public string productName { get; set; }
    public DateTime uploadedate { get; set; }
    public string productUrl { get; set; }
    public string creatorNames { get; set; }
    public DateTime uploaded { get; set; }
    public string sdl_extracted_summary { get; set; }
}
public class PWS : Insight {
    public string sdl_source_type = "PWS";
    public string source_library { get; set; }
    public string file_name { get; set; }
    public string document_url { get; set; }
    public string uploaded_by { get; set; }
    public string last_modified { get; set; }

}
public class Platform : Insight {
    public string sdl_source_type = "platform";
    public DateTime field_launch_date { get; set; }
    public DateTime changed { get; set; }
    public string field_platform_contacts { get; set; }
    public string field_communities_of_practice { get; set; }
    public string platform_leader_name { get; set; }
    public string field_banner_subhead { get; set; }
    public string platform_url { get; set; }
}
public class PRC : Insight {
    public string sdl_source_type = "PRC";
}
public class MIP : Insight {
    public string sdl_source_type = "MIP Projects";
    public string chargeCode { get; set; }
    public string longName { get; set; }
    public DateTime endDate { get; set; }
    public string phonebookDisplayName { get; set; }
    public DateTime startDate { get; set; }
    public string status { get; set; }
    public string portfolios { get; set; }
    public string name { get; set; }
    public string public_url { get; set; }
    public string project_url { get; set; }
}
public class MVC : Insight {
    public string sdl_source_type = "MVC";
    public string project_name { get; set; }
    public string project_sponsor { get; set; }
    public DateTime project_end { get; set; }
    public string portfolio { get; set; }
    public string super_portfolio { get; set; }
    public string sub_portfolio { get; set; }
    public string clarify { get; set; }
    public string project_url { get; set; }
    public string project_page_charge_code { get; set; }
    public string project_leader {get;set;}
}

public class tcas : Insight {
    public string sdl_source_type = "tcas";
    public string field_tca_short_name { get; set; }
    public DateTime changed { get; set; }
    public string field_tca_organizationleadername { get; set; }
    public string capability_rul { get; set; }
}

public class Insight {
    public DateTime sdl_date { get; set; }
    public string countryPublished { get; set; }
    public string conference { get; set; }
    public string originalAuthorName { get; set; }
    public string title { get; set; }
    public string declaredTags { get; set; }
    public string releaseReason { get; set; }
    public string docName { get; set; }
    public int fundingCenter { get; set; }
    public string resourceURL { get; set; }
    public string fundingDepartment { get; set; }
    public string caseNumber { get; set; }
    public string publicationDate { get; set; }
    public int releaseYear { get; set; }
    public string releaseStatement { get; set; }
    public string approver { get; set; }
    public int handCarry { get; set; }
    public string authorDivision { get; set; }
    public string copyrightOwner { get; set; }
    public string lastModifiedDate { get; set; }
    public string releaseDate { get; set; }
    public int onMitrePublicSrvr { get; set; }
    public string projectNumber { get; set; }
    public string materialType { get; set; }
    public string publicationType { get; set; }
    public int authorCenter { get; set; }
    public string originalAuthorID { get; set; }
    public int mitrePublicServer { get; set; }
    public string subjectTerminology { get; set; }
    public string dateEntered { get; set; }
    public string documentInfoURL { get; set; }
    public int softShell { get; set; }
    public int publishedOnNonMITREServer { get; set; }
    public string priorCaseNumbers { get; set; }
    public string organization { get; set; }
    public string authorDepartment { get; set; }
    public int publicationYear { get; set; }
    public string sensitivity { get; set; }
    public string copyrightText { get; set; }
    public string fundingSource { get; set; }
    public string level1 { get; set; }
    public string fundingDivision { get; set; }
    public int publishedOutsideUSA { get; set; }
    public string level3 { get; set; }
    public string level2 { get; set; }
    public string sdl_id { get; set; }
    public string text { get; set; }
    public string updated_at { get; set; }
    public string created_at { get; set; }
}