using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;

namespace AwsDotnetCsharp
{
    public class Handler
    {
        private const string PLEDGE_ID = "pledgeID";
        private const string IS_CLEARED = "is_cleared";
        private const string TABLE_NAME = "offset-trump";

        private static async Task<ScanResponse> FetchPledgesAsync()
        {
            var client = new AmazonDynamoDBClient();
            var response = await client.ScanAsync(TABLE_NAME, new List<string> { PLEDGE_ID, IS_CLEARED });
            return response;
        }

        public async Task<Response> GetPledges(Request request)
        {
            var response = await FetchPledgesAsync();
            var pledges = response.Items.Select(item => new
            {
                Pledge = item[IS_CLEARED].BOOL ? item[PLEDGE_ID].S : "Pending moderation"
            }).ToList();
            return new Response(pledges);
        }

        public async Task<Response> AdminGetPledges(Request request)
        {
            if (!request.IsAuthorized())
            {
                return new Response(new List<object>());
            }
            var response = await FetchPledgesAsync();
            var pledges = response.Items
                .Where(item => !item[IS_CLEARED].BOOL)
                .Select(item => new
                {
                    Pledge = item[PLEDGE_ID].S
                })
                .ToList();
            return new Response(pledges);
        }

        public async Task<Response> EditPledge(Request request)
        {
            var pledgeText = ExtractPledge(request.Body);
            await PutPledgeAsync(pledgeText, isCleared: true);
            return new Response();
        }

        public async Task<Response> DeletePledge(Request request)
        {
            var pledgeText = ExtractPledge(request.Body);
            await PutPledgeAsync(pledgeText, isCleared: true);
            var client = new AmazonDynamoDBClient();
            await DeletePledgeAsync(pledgeText);
            return new Response();
        }

        public async Task<Response> CreatePledge(Request request)
        {
            var pledgeText = ExtractPledge(request.Body);
            var lowerCasePledge = pledgeText.ToLower();
            if (!Helpers.Naughties.Any(n => lowerCasePledge.Contains(n)))
            {
                await PutPledgeAsync(pledgeText, isCleared: false);
            }
            return new Response();
        }

        private async Task PutPledgeAsync(string pledgeText, bool isCleared)
        {
                var client = new AmazonDynamoDBClient();
                var table = Table.LoadTable(client, TABLE_NAME);
                var pledgeDocument = new Document();
                pledgeDocument[PLEDGE_ID] = pledgeText;
                pledgeDocument[IS_CLEARED] = new DynamoDBBool(isCleared);
                await table.PutItemAsync(pledgeDocument);
        }

        private async Task DeletePledgeAsync(string pledgeText)
        {
                var client = new AmazonDynamoDBClient();
                var table = Table.LoadTable(client, TABLE_NAME);
                var pledgeDocument = new Document();
                pledgeDocument[PLEDGE_ID] = pledgeText;
                await table.DeleteItemAsync(pledgeDocument);
        }

        private static string ExtractPledge(string body)
        {
            var extractedFormData = body.Split('\n')[3];
            return extractedFormData.Replace("\"", "");
        }
    }

    public class Request
    {
        public const string ADMIN_KEY = "CHANGEME";

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("pathParameters")]
        public Dictionary<string,string> PathParameters { get; set; }

        public bool IsAuthorized()
        {
            if (PathParameters.ContainsKey("key") && PathParameters["key"] == ADMIN_KEY)
            {
                return true;
            }
            return false;
        }
    }

    public class Response
    {
        public Response()
        { }

        public Response(object body)
        {
            Body = JsonConvert.SerializeObject(body);
        }
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; } = 200;
        [JsonProperty("headers")]
        public Dictionary<string, string> Headers { get; set; }
            = new Dictionary<string, string> { { "Access-Control-Allow-Origin", "*"}};
        [JsonProperty("body")]
        public string Body { get; set; }
    }

    public static class Helpers
    {
        public static List<string> Naughties = new List<string>
        {
            "4r5e",
            "5h1t",
            "5hit",
            "a55",
            "anal",
            "anus",
            "ar5e",
            "arrse",
            "arse",
            "ass",
            "ass-fucker",
            "asses",
            "assfucker",
            "assfukka",
            "asshole",
            "assholes",
            "asswhole",
            "a_s_s",
            "b!tch",
            "b00bs",
            "b17ch",
            "b1tch",
            "ballbag",
            "balls",
            "ballsack",
            "bastard",
            "beastial",
            "beastiality",
            "bellend",
            "bestial",
            "bestiality",
            "bi+ch",
            "biatch",
            "bitch",
            "bitcher",
            "bitchers",
            "bitches",
            "bitchin",
            "bitching",
            "bloody",
            "blow job",
            "blowjob",
            "blowjobs",
            "boiolas",
            "bollock",
            "bollok",
            "boner",
            "boob",
            "boobs",
            "booobs",
            "boooobs",
            "booooobs",
            "booooooobs",
            "breasts",
            "buceta",
            "bugger",
            "bum",
            "bunny fucker",
            "butt",
            "butthole",
            "buttmuch",
            "buttplug",
            "c0ck",
            "c0cksucker",
            "carpet muncher",
            "cawk",
            "chink",
            "cipa",
            "cl1t",
            "clit",
            "clitoris",
            "clits",
            "cnut",
            "cock",
            "cock-sucker",
            "cockface",
            "cockhead",
            "cockmunch",
            "cockmuncher",
            "cocks",
            "cocksuck ",
            "cocksucked ",
            "cocksucker",
            "cocksucking",
            "cocksucks ",
            "cocksuka",
            "cocksukka",
            "cok",
            "cokmuncher",
            "coksucka",
            "coon",
            "cox",
            "crap",
            "cum",
            "cummer",
            "cumming",
            "cums",
            "cumshot",
            "cunilingus",
            "cunillingus",
            "cunnilingus",
            "cunt",
            "cuntlick ",
            "cuntlicker ",
            "cuntlicking ",
            "cunts",
            "cyalis",
            "cyberfuc",
            "cyberfuck ",
            "cyberfucked ",
            "cyberfucker",
            "cyberfuckers",
            "cyberfucking ",
            "d1ck",
            "damn",
            "dick",
            "dickhead",
            "dildo",
            "dildos",
            "dink",
            "dinks",
            "dirsa",
            "dlck",
            "dog-fucker",
            "doggin",
            "dogging",
            "donkeyribber",
            "doosh",
            "duche",
            "dyke",
            "ejaculate",
            "ejaculated",
            "ejaculates ",
            "ejaculating ",
            "ejaculatings",
            "ejaculation",
            "ejakulate",
            "f u c k",
            "f u c k e r",
            "f4nny",
            "fag",
            "fagging",
            "faggitt",
            "faggot",
            "faggs",
            "fagot",
            "fagots",
            "fags",
            "fanny",
            "fannyflaps",
            "fannyfucker",
            "fanyy",
            "fatass",
            "fcuk",
            "fcuker",
            "fcuking",
            "feck",
            "fecker",
            "felching",
            "fellate",
            "fellatio",
            "fingerfuck ",
            "fingerfucked ",
            "fingerfucker ",
            "fingerfuckers",
            "fingerfucking ",
            "fingerfucks ",
            "fistfuck",
            "fistfucked ",
            "fistfucker ",
            "fistfuckers ",
            "fistfucking ",
            "fistfuckings ",
            "fistfucks ",
            "flange",
            "fook",
            "fooker",
            "fuck",
            "fucka",
            "fucked",
            "fucker",
            "fuckers",
            "fuckhead",
            "fuckheads",
            "fuckin",
            "fucking",
            "fuckings",
            "fuckingshitmotherfucker",
            "fuckme ",
            "fucks",
            "fuckwhit",
            "fuckwit",
            "fudge packer",
            "fudgepacker",
            "fuk",
            "fuker",
            "fukker",
            "fukkin",
            "fuks",
            "fukwhit",
            "fukwit",
            "fux",
            "fux0r",
            "f_u_c_k",
            "gangbang",
            "gangbanged ",
            "gangbangs ",
            "gaylord",
            "gaysex",
            "goatse",
            "God",
            "god-dam",
            "god-damned",
            "goddamn",
            "goddamned",
            "hardcoresex ",
            "hell",
            "heshe",
            "hoar",
            "hoare",
            "hoer",
            "homo",
            "hore",
            "horniest",
            "horny",
            "hotsex",
            "jack-off ",
            "jackoff",
            "jap",
            "jerk-off ",
            "jism",
            "jiz ",
            "jizm ",
            "jizz",
            "kawk",
            "knob",
            "knobead",
            "knobed",
            "knobend",
            "knobhead",
            "knobjocky",
            "knobjokey",
            "kock",
            "kondum",
            "kondums",
            "kum",
            "kummer",
            "kumming",
            "kums",
            "kunilingus",
            "l3i+ch",
            "l3itch",
            "labia",
            "lmfao",
            "lust",
            "lusting",
            "m0f0",
            "m0fo",
            "m45terbate",
            "ma5terb8",
            "ma5terbate",
            "masochist",
            "master-bate",
            "masterb8",
            "masterbat*",
            "masterbat3",
            "masterbate",
            "masterbation",
            "masterbations",
            "masturbate",
            "mo-fo",
            "mof0",
            "mofo",
            "mothafuck",
            "mothafucka",
            "mothafuckas",
            "mothafuckaz",
            "mothafucked ",
            "mothafucker",
            "mothafuckers",
            "mothafuckin",
            "mothafucking ",
            "mothafuckings",
            "mothafucks",
            "mother fucker",
            "motherfuck",
            "motherfucked",
            "motherfucker",
            "motherfuckers",
            "motherfuckin",
            "motherfucking",
            "motherfuckings",
            "motherfuckka",
            "motherfucks",
            "muff",
            "mutha",
            "muthafecker",
            "muthafuckker",
            "muther",
            "mutherfucker",
            "n1gga",
            "n1gger",
            "nazi",
            "nigg3r",
            "nigg4h",
            "nigga",
            "niggah",
            "niggas",
            "niggaz",
            "nigger",
            "niggers ",
            "nob",
            "nob jokey",
            "nobhead",
            "nobjocky",
            "nobjokey",
            "numbnuts",
            "nutsack",
            "orgasim ",
            "orgasims ",
            "orgasm",
            "orgasms ",
            "p0rn",
            "pawn",
            "pecker",
            "penis",
            "penisfucker",
            "phonesex",
            "phuck",
            "phuk",
            "phuked",
            "phuking",
            "phukked",
            "phukking",
            "phuks",
            "phuq",
            "pigfucker",
            "pimpis",
            "piss",
            "pissed",
            "pisser",
            "pissers",
            "pisses ",
            "pissflaps",
            "pissin ",
            "pissing",
            "pissoff ",
            "poop",
            "porn",
            "porno",
            "pornography",
            "pornos",
            "prick",
            "pricks ",
            "pron",
            "pube",
            "pusse",
            "pussi",
            "pussies",
            "pussy",
            "pussys ",
            "rectum",
            "retard",
            "rimjaw",
            "rimming",
            "s hit",
            "s.o.b.",
            "sadist",
            "schlong",
            "screwing",
            "scroat",
            "scrote",
            "scrotum",
            "semen",
            "sex",
            "sh!+",
            "sh!t",
            "sh1t",
            "shag",
            "shagger",
            "shaggin",
            "shagging",
            "shemale",
            "shi+",
            "shit",
            "shitdick",
            "shite",
            "shited",
            "shitey",
            "shitfuck",
            "shitfull",
            "shithead",
            "shiting",
            "shitings",
            "shits",
            "shitted",
            "shitter",
            "shitters ",
            "shitting",
            "shittings",
            "shitty ",
            "skank",
            "slut",
            "sluts",
            "smegma",
            "smut",
            "snatch",
            "son-of-a-bitch",
            "spac",
            "spunk",
            "s_h_i_t",
            "t1tt1e5",
            "t1tties",
            "teets",
            "teez",
            "testical",
            "testicle",
            "tit",
            "titfuck",
            "tits",
            "titt",
            "tittie5",
            "tittiefucker",
            "titties",
            "tittyfuck",
            "tittywank",
            "titwank",
            "tosser",
            "turd",
            "tw4t",
            "twat",
            "twathead",
            "twatty",
            "twunt",
            "twunter",
            "v14gra",
            "v1gra",
            "vagina",
            "viagra",
            "vulva",
            "w00se",
            "wang",
            "wank",
            "wanker",
            "wanky",
            "whoar",
            "whore",
            "willies",
            "willy",
            "xrated",
            "xxx",
        };
    }
}
