using System;
using System.Collections.Generic;
using System.Linq;

namespace UPFC
{
    class Program
    {
        //TODO: add multi-fusion
        //TODO: add treasure demons
        //TODO: take the cheapest option from all possible ones
        //TODO: make the program try every single permutation of the given skill list
        //TODO: put the personas and skills in a file
        static Dictionary<string, string> SDB = new Dictionary<string, string>();
        static List<Persona> PDB = new List<Persona>();
        static List<string> skilllist = new List<string>(), output = new List<string>();
        static Persona endgoal;
        static Queue<Persona> queue = new Queue<Persona>();
        static int[,] arcanaTable = new int[,]
        {
                { 0, 13, 18, 12, 14, 9, 7, 18, 17, 2, 6, 13, 16, 11, 5, 14, 3, 1, 8, 8, 19 },
                { 13, 1, 14, 8, 12, 13, 15, 2, 4, 6, 8, 0, 3, 9, 7, 5, 14, 2, 6, 5, 11},
                { 18, 14, 2, 4, 3, 1, 10, 5, 13, 14, 1, 15, 13, 1, 15, 18, 12, 9, 5, 7, 8},
                { 12, 8, 4, 3, 8, 0, 20, 17, 6, 11, 9, 7, 2, 0, 2, 19, 3, 6, 10, 16, 4},
                { 14, 12, 3, 8, 4, 10, 0, 11, 7, 5, 19, 16, 15, 9, 15, 8, 17, 6, 16, 20, 2},
                { 9, 13, 1, 0, 10, 5, 11, 17, 12, 10, 8, 0, 19, 7, 13, 12, 20, 16, 2, 6, 4},
                { 7, 15, 10, 20, 0, 11, 6, 14, 20, 7, 11, 13, 19, 14, 11, 18, 3, 7, 1, 3, 12},
                { 18, 2, 5, 17, 11, 17, 14, 7, 18, 15, 2, 9, 0, 15, 11, 14, 10, 18, 6, 2, -1},
                { 17, 4, 13, 6, 7, 12, 20, 18, 8, 1, 4, 5, 6, 0, 4, 0, 19, 3, 15, 12, -1},
                { 2, 6, 14, 11, 5, 10, 7, 15, 1, 9, 17, 5, 17, 11, 11, 2, 20, 11, 2, 15, 4},
                { 6, 8, 1, 9, 19, 8, 11, 2, 4, 17, 10, 14, 4, 17, 3, 5, 12, 15, 19, 17, 16},
                { 13, 0, 15, 7, 16, 0, 13, 9, 5, 5, 14, 11, 14, 5, 7, 13, 7, 18, 1, 18, -1},
                { 16, 3, 13, 2, 15, 19, 19, 0, 6, 17, 4, 14, 12, 18, 13, 10, 9, 8, 11, 5, 17},
                { 11, 9, 1, 0, 9, 7, 14, 15, 0, 11, 17, 5, 18, 13, 12, 7, 19, 15, 5, 2, -1},
                { 5, 7, 15, 2, 15, 13, 11, 11, 4, 11, 3, 7, 13, 12, 14, 0, 10, 19, 10, 1, 9},
                { 14, 5, 18, 19, 8, 12, 18, 14, 0, 2, 5, 13, 10, 7, 0, 15, 1, 11, 7, 9, 6},
                { 3, 14, 12, 4, 17, 20, 3, 10, 19, 20, 12, 7, 9, 19, 10, 1, 16, 13, 9, 4, 18},
                { 1, 2, 9, 6, 6, 16, 7, 18, 3, 11, 15, 18, 8, 15, 19, 11, 13, 17, 14, 20, 10},
                { 8, 6, 5, 10, 16, 2, 1, 6, 15, 2, 19, 1, 11, 5, 10, 7, 9, 14, 18, 3, 0},
                { 8, 5, 7, 16, 20, 6, 3, 2, 12, 15, 17, 18, 5, 2, 1, 9, 4, 20, 3, 19, 13},
                { 19, 11, 8, 4, 2, 4, 12, -1, -1, 4, 16, -1, 17, -1, 9, 6, 18, 10, 0, 13, 20}
            };
        static Dictionary<int, string> arcanaDict = new Dictionary<int, string>();
        static Dictionary<Persona[], Persona> pairList = new Dictionary<Persona[], Persona>();

        static void Main(string[] args)
        {
            FillDatabase();
            //TestData();
            RequestInput();
            checkEndMoveset();
            if (skilllist.Any())
                BFS(skilllist[0], 0, endgoal);
            else
                TrackChain(FindRecipes(endgoal)[0], endgoal);
            //if there are no results
            if (!output.Any())
                Console.WriteLine("No results found. Either none of the selected skills can be fused onto the desired Persona, or you misspelled one or more skill names. Be aware of proper capitalization.");
            output.Reverse();
            foreach (string s in output)
            {
                Console.WriteLine(s);
            }
            Console.ReadLine();
        }

        static void checkEndMoveset()
        {
            List<string> output = new List<string>();
            foreach (string skill in skilllist)
                if (!endgoal.moveset.Contains(skill))
                    output.Add(skill);
            skilllist = output;
        }

        static void RequestInput()
        {
            string input;
            Console.WriteLine("Name of Persona:");
            input = Console.ReadLine();
            endgoal = PDB.FirstOrDefault(p => p.name == input);
            if (!PDB.Any(a => a.name == endgoal.name))
            {
                Console.WriteLine("That Persona does not exist. Did you spell the name correctly? Be aware of proper capitalization.");
                RequestInput();
                return;
            }
            for (int i = 1; i <= 8; i++)
            {
                Console.WriteLine("Skill " + i + ":");
                input = Console.ReadLine();
                if (input == "")
                    break;
                skilllist.Add(input);
            }
        }

        static void TestData()
        {
            foreach (Persona p in PDB)
            {
                foreach (string s in p.moveset)
                    if (!SDB.ContainsKey(s))
                        Console.WriteLine("Skill " + s + " on persona " + p.name + " does not exist in the skill database.");
                if (p.restrictions.Count == 0)
                {
                    Console.WriteLine("Persona " + p.name + " is Almighty type");
                }
            }
        }

        static void BFS(string s, int counter, Persona target)
        {
            counter++;
            queue = new Queue<Persona>();
            queue.Enqueue(target);
            while (queue.Count > 0)
            {
                Persona u = queue.Dequeue();
                Persona[] sl = FindRecipes(u);
                try
                {
                    foreach (Persona p in sl)
                        try
                        {
                            if (!p.restrictions.Contains(SDB[s]))
                            {
                                if (p.moveset.Contains(s))
                                {
                                    TrackChain(p, target);
                                    pairList.Clear();
                                    if (counter < skilllist.Count)
                                        BFS(skilllist[counter], counter, p);
                                    return;
                                }
                                queue.Enqueue(p);
                            }
                        }
                        catch (KeyNotFoundException) { }
                }
                catch (NullReferenceException) { }
            }
        }

        static void TrackChain(Persona input, Persona target)
        {
            foreach (KeyValuePair<Persona[], Persona> fusion in pairList)
            {
                if (fusion.Key.Contains(input))
                {
                    if (fusion.Value != target)
                        TrackChain(fusion.Value, target);
                    output.Add("Fuse " + fusion.Key[0].name + " and " + fusion.Key[1].name + " into " + fusion.Value.name + ".");
                    return;
                }
            }
        }

        static Persona[] FindRecipes(Persona u)
        {
            //all possible personas that can be used to fuse u
            List<Persona> output = new List<Persona>();
            for (int i = 0; i < 21; i++)
                for (int j = 0; j < 21; j++)
                    //check which combination of arcanas can create the input's arcana
                    if (arcanaTable[i, j] == u.arcana)
                        foreach (Persona p1 in PDB.Where(p1 => p1.arcana == i && p1 != u))
                            foreach (Persona p2 in PDB.Where(p2 => p2.arcana == j && p2 != u && p2 != p1))
                                if (ClosestPersona(u.arcana, ((p1.level + p2.level) / 2 + 1), p1, p2) == u)
                                {
                                    pairList.Add(new Persona[] { p1, p2 }, u);
                                    output.Add(p1);
                                    output.Add(p2);
                                }
            return output.ToArray();
        }

        static Persona ClosestPersona(int arcana, int level, Persona P1, Persona P2)
        {
            Persona minValid = null;
            foreach (Persona pr in PDB.Where(pr => pr.arcana == arcana))
                if ((minValid == null) || (Math.Abs(pr.level - level) < Math.Abs(level - minValid.level) && ((P1 != pr) || (P2 != pr))))
                    minValid = pr;
            return minValid;
        }

        static void FillDatabase()
        {
            FillMoves();
            FillArcana();
            //name, type, arcana, level, moveset
            PDB.Add(new Persona("Arsene", "Curse", 0, 1, new List<string>() { "Eiha", "Cleave", "Sukunda", "Dream Needle", "Adverse Resolve" }));
            PDB.Add(new Persona("Jack-o'-Lantern", "Fire", 1, 2, new List<string>() { "Agi", "Rakunda", "Sharp Student", "Dazzler", "Resist Sleep" }));
            PDB.Add(new Persona("Pixie", "Lightning", 6, 2, new List<string>() { "Zio", "Dia", "Patra", "Tarukaja", "Resist Confuse" }));
            PDB.Add(new Persona("Agathion", "Lightning", 7, 3, new List<string>() { "Zio", "Baisudi", "Dia", "Lunge", "Rakukaja", "Dodge Elec" }));
            PDB.Add(new Persona("Mandrake", "Lightning", 13, 3, new List<string>() { "Pulinpa", "Energy Drop", "Lunge", "Sukunda", "Skull Cracker" }));
            PDB.Add(new Persona("Bicorn", "Wind", 9, 4, new List<string>() { "Lunge", "Tarunda", "Garu", "Ice Wall", "Apt Pupil" }));
            PDB.Add(new Persona("Incubus", "Ailment", 15, 5, new List<string>() { "Evil Touch", "Life Drain", "Eiha", "Tarunda", "Dodge Curse" }));
            PDB.Add(new Persona("Silky", "Healing", 2, 6, new List<string>() { "Bufu", "Dormina", "Dia", "Patra", "Sharp Student" }));
            PDB.Add(new Persona("Saki Mitama", "Healing", 6, 6, new List<string>() { "Bufu", "Energy Drop", "Wind Wall", "Growth 1", "Rakukaja", "Resist Dizzy" }));
            PDB.Add(new Persona("Kelpie", "Wind", 11, 6, new List<string>() { "Lunge", "Garu", "Resist Brainwash", "Sukukaja", "Terror Claw" }));
            PDB.Add(new Persona("Genbu", "Ice", 14, 7, new List<string>() { "Bufu", "Rakunda", "Patra", "Mabufu", "Resist Forget", "Defense Master" }));
            PDB.Add(new Persona("Succubus", "Curse", 18, 7, new List<string>() { "Marin Karin", "Rebellion", "Agi", "Dekaja", "Brainwash Boost", "Mudo" }));
            PDB.Add(new Persona("Obariyon", "Physical", 0, 8, new List<string>() { "Snap", "Sukunda", "Lucky Punch", "Resist Fear", "Dekaja" }));
            PDB.Add(new Persona("Berith", "Physical", 5, 9, new List<string>() { "Cleave", "Rakukaja", "Double Fangs", "Dodge Fire", "Sledgehammer" }));
            PDB.Add(new Persona("Koropokkuru", "Ice", 9, 9, new List<string>() { "Bufu", "Makajam", "Dodge Ice", "Rakunda", "Fire Wall", "Mabufu" }));
            PDB.Add(new Persona("Hua Po", "Fire", 12, 9, new List<string>() { "Agi", "Dormina", "Tarunda", "Resist Forget", "Maragi", "Burn Boost" }));
            PDB.Add(new Persona("Mokoi", "Ailment", 13, 9, new List<string>() { "Skull Cracker", "Dazzler", "Tarukaja", "Marin Karin", "Dodge Elec", "Dekunda" }));
            PDB.Add(new Persona("Slime", "Curse", 7, 10, new List<string>() { "Lunge", "Evil Touch", "Eiha", "Fire Wall", "Headbutt" }));
            PDB.Add(new Persona("Andras", "Ice", 15, 10, new List<string>() { "Bufu", "Rakunda", "Tarukaja", "Apt Pupil", "Mabufu", "Ice Break" }));
            PDB.Add(new Persona("Jack Frost", "Ice", 1, 11, new List<string>() { "Bufu", "Baisudi", "Ice Break", "Mabufu", "Rakunda", "Freeze Boost" }));
            PDB.Add(new Persona("Apsaras", "Ice", 2, 11, new List<string>() { "Bufu", "Ice Wall", "Rebellion", "Media", "Elec Wall", "Wind Wall" }));
            PDB.Add(new Persona("Koppa Tengu", "Wind", 14, 11, new List<string>() { "Snap", "Garu", "Growth 1", "Taunt", "Rage Boost", "Wage War" }));
            PDB.Add(new Persona("Kodama", "Ailment", 17, 11, new List<string>() { "Garu", "Rakunda", "Psi", "Evil Touch", "Tarukaja", "Fear Boost", "Resist Fear" }));
            PDB.Add(new Persona("Angel", "Bless", 8, 12, new List<string>() { "Hama", "Dazzler", "Dia", "Kouha", "Baisudi", "Dodge Curse", "Dekunda" }));
            PDB.Add(new Persona("Onmoraki", "Curse", 18, 12, new List<string>() { "Mudo", "Ice Wall", "Agi", "Pulinpa", "Confuse Boost", "Resist Fear" }));
            PDB.Add(new Persona("Ippon-Datara", "Physical", 9, 13, new List<string>() { "Sledgehammer", "Tarukaja", "Resist Dizzy", "Rampage", "Sharp Student", "Counter" }));
            PDB.Add(new Persona("Kushi Mitama", "Healing", 11, 14, new List<string>() { "Bufu", "Garu", "Makajam", "Media", "Regenerate 1", "Wind Wall", "Forget Boost" }));
            PDB.Add(new Persona("Inugami", "Fire", 12, 14, new List<string>() { "Giant Slice", "Pulinpa", "Tarukaja", "Dream Needle", "Lucky Punch", "Brain Shake", "Confuse Boost" }));
            PDB.Add(new Persona("Makami", "Nuclear", 14, 15, new List<string>() { "Double Fangs", "Frei", "Energy Drop", "Mafrei", "Makajam", "Resist Despair", "Dodge Elec" }));
            PDB.Add(new Persona("High Pixie", "Healing", 0, 16, new List<string>() { "Garu", "Dormina", "Media", "Diarama", "Taunt", "Magaru" }));
            PDB.Add(new Persona("Eligor", "Fire", 4, 16, new List<string>() { "Maragi", "Tarukaja", "Sharp Student", "Double Fangs", "Sukunda", "Memory Blow" }));
            PDB.Add(new Persona("Shiisaa", "Lightning", 7, 16, new List<string>() { "Double Fangs", "Skull Cracker", "Zio", "Rampage", "Dodge Curse", "Dodge Elec" }));
            PDB.Add(new Persona("Archangel", "Bless", 8, 16, new List<string>() { "Psi", "Hama", "Dazzler", "Rebellion", "Makouha", "Vajra Blast" }));
            PDB.Add(new Persona("Nekomata", "Ailment", 1, 17, new List<string>() { "Terror Claw", "Magaru", "Evil Touch", "Hysterical Slap", "Wind Break", "Elec Wall", "Dodge Elec" }));
            PDB.Add(new Persona("Orobas", "Fire", 5, 17, new List<string>() { "Maragi", "Dekaja", "Sukukaja", "Marakunda", "Fire Break", "Makajamaon" }));
            PDB.Add(new Persona("Sudama", "Wind", 9, 17, new List<string>() { "Lucky Punch", "Magaru", "Sharp Student", "Ambient Aid", "Wind Wall", "Apt Pupil", "Garula" }));
            PDB.Add(new Persona("Leanan Sidhe", "None", 6, 19, new List<string>() { "Psio", "Rakunda", "Marin Karin", "Mamudo", "Mapsi", "Eiga" }));
            PDB.Add(new Persona("Oni", "Physical", 11, 19, new List<string>() { "Rampage", "Snap", "Counter", "Giant Slice", "Sharp Student", "Memory Blow" }));
            PDB.Add(new Persona("Matador", "Psy", 13, 19, new List<string>() { "Psi", "Sukukaja", "Null Dizzy", "Swift Strike", "Trigger Happy", "Garula" }));
            PDB.Add(new Persona("Suzaku", "Nuclear", 19, 19, new List<string>() { "Frei", "Marin Karin", "Tarunda", "Ominous Words", "Mafrei", "Speed Master", "Matarunda" }));
            PDB.Add(new Persona("Yaksini", "Ice", 3, 20, new List<string>() { "Hysterical Slap", "Wage War", "Counter", "Oni Kagura", "Attack Master", "Vicious Strike" }));
            PDB.Add(new Persona("Nigi Mitama", "Healing", 14, 20, new List<string>() { "Makouha", "Baisudi", "Media", "Divine Grace", "Me Patra", "Rainy Play" }));
            PDB.Add(new Persona("Nue", "Curse", 18, 20, new List<string>() { "Skull Cracker", "Maeiha", "Mudo", "Pulinpa", "Mamudo", "Assault Dive", "Curse Boost" }));
            PDB.Add(new Persona("Shiki-Ouji", "Psy", 7, 21, new List<string>() { "Snap", "Taunt", "Tarukaja", "Mapsi", "Dekaja", "Psio", "Oni Kagura" }));
            PDB.Add(new Persona("Orthrus", "Fire", 12, 21, new List<string>() { "Double Fangs", "Agilao", "Dodge Ice", "Burn Boost", "Rat Fang", "Matarukaja" }));
            PDB.Add(new Persona("Phoenix", "Nuclear", 5, 22, new List<string>() { "Dream Needle", "Freila", "Diarama", "Recarm", "Nuke Boost" }));
            PDB.Add(new Persona("Fuu-Ki", "Wind", 17, 23, new List<string>() { "Garula", "Tarukaja", "Tetra Break", "Wind Boost", "Dodge Wind", "Resist Psy" }));
            PDB.Add(new Persona("Sandman", "Wind", 1, 24, new List<string>() { "Dormin Rush", "Garula", "Dormina", "Sukunda", "Null Sleep", "Magarula", "Sleep Boost" }));
            PDB.Add(new Persona("Naga", "Lightning", 9, 24, new List<string>() { "Double Fangs", "Memory Blow", "Zionga", "Elec Boost", "Dazzler", "Mazionga", "Marakukaja" }));
            PDB.Add(new Persona("Rakshasa", "Physical", 11, 24, new List<string>() { "Giant Slice", "Tarukaja", "Wind Wall", "Regenerate 1", "Mind Slice", "Counterstrike", "Adverse Resolve" }));
            PDB.Add(new Persona("Sui-Ki", "Ice", 18, 24, new List<string>() { "Headbutt", "Bufula", "Mabufu", "Null Nuke", "Wage War", "Mabufula", "Dodge Fire" }));
            PDB.Add(new Persona("Anzu", "Wind", 5, 25, new List<string>() { "Garula", "Masukukaja", "Wind Break", "Assault Dive", "Dekaja", "Null Forget" }));
            PDB.Add(new Persona("Kin-Ki", "Physical", 7, 25, new List<string>() { "Vajra Blast", "Rakukaja", "Regenerate 1", "Dodge Psy", "Sledgehammer", "Bad Beat", "Counterstrike" }));
            PDB.Add(new Persona("Jikokuten", "Physical", 14, 25, new List<string>() { "Memory Blow", "Rakunda", "Defense Master", "Dekunda", "Counter", "Matarukaja", "Adverse Resolve" }));
            PDB.Add(new Persona("Isis", "Healing", 2, 26, new List<string>() { "Agilao", "Diarama", "Sukukaja", "Resist Forget", "Zionga", "Garula", "Makarakarn" }));
            PDB.Add(new Persona("Lamia", "Fire", 3, 26, new List<string>() { "Rising Slash", "Agilao", "Rakukaja", "Ominous Words", "Foul Breath", "Maragion", "Despair Boost" }));
            PDB.Add(new Persona("Clotho", "Healing", 10, 26, new List<string>() { "Mahama", "Makajam", "Me Patra", "Tetraja", "Makajamaon", "Energy Shower", "Invigorate 1" }));
            PDB.Add(new Persona("Choronzon", "Curse", 1, 28, new List<string>() { "Rampage", "Pulinpa", "Life Drain", "Maeiha", "Dodge Elec", "Eiga", "Curse Boost", "Rainy Play" }));
            PDB.Add(new Persona("Setanta", "Physical", 4, 28, new List<string>() { "Dormin Rush", "Giant Slice", "Counter", "Rising Slash", "Rebellion", "Charge" }));
            PDB.Add(new Persona("Ame-no-Uzume", "None", 6, 29, new List<string>() { "Bufula", "Mazio", "Diarama", "Tentafaroo", "Divine Grace", "Shock Boost" }));
            PDB.Add(new Persona("Principality", "Bless", 8, 29, new List<string>() { "Makouga", "Makajamaon", "Tetraja", "Mediarama", "Mabaisudi", "Bless Boost" }));
            PDB.Add(new Persona("Take-Minakata", "Lightning", 12, 29, new List<string>() { "Assault Dive", "Zionga", "Elec Break", "Mazionga", "Defense Master", "Elec Boost" }));
            PDB.Add(new Persona("Picasa", "Curse", 13, 29, new List<string>() { "Dream Needle", "Rampage", "Stagnant Air", "Mamudo", "Abysmal Surge", "Despair Boost", "Mudoon" }));
            PDB.Add(new Persona("Black Ooze", "Curse", 18, 29, new List<string>() { "Evil Touch", "Stagnant Air", "Matarunda", "Ambient Aid", "Headbutt", "Brain Jack", "Flash Bomb" }));
            PDB.Add(new Persona("Ara Mitama", "Nuclear", 7, 31, new List<string>() { "Miracle Punch", "Freila", "Taunt", "Rebellion", "Marakunda", "Rage Boost" }));
            PDB.Add(new Persona("Zouchouten", "Lightning", 11, 31, new List<string>() { "Giant Slice", "Zionga", "Terror Claw", "Sharp Student", "Resist Fear", "Swift Strike", "Attack Master" }));
            PDB.Add(new Persona("Decarabia", "Fire", 0, 32, new List<string>() { "Agilao", "Ominous Words", "Maragion", "Fire Boost", "Null Fire", "Evil Smile", "Tetrakarn" }));
            PDB.Add(new Persona("Lilim", "Ice", 15, 32, new List<string>() { "Bufula", "Evil Smile", "Masukunda", "Freeze Boost", "Dodge Bless", "Spirit Drain", "Mabufula" }));
            PDB.Add(new Persona("Jatayu", "Wind", 16, 32, new List<string>() { "Flash Bomb", "Garula", "Masukukaja", "Dodge Psy", "Snipe", "Rainy Play", "Speed Master" }));
            PDB.Add(new Persona("Mithra", "Bless", 14, 33, new List<string>() { "Kouga", "Mahama", "Diarama", "Makouga", "Dekunda", "Bless Boost", "Thermopylae" }));
            PDB.Add(new Persona("Mothman", "Lightning", 18, 33, new List<string>() { "Skull Cracker", "Mazionga", "Shock Boost", "Tentafaroo", "Ambient Aid", "Makajamaon" }));
            PDB.Add(new Persona("Lachesis", "Ice", 10, 34, new List<string>() { "Bufula", "Mabaisudi", "Growth 2", "Marakukaja", "Elec Wall", "Mabufula", "Ice Boost" }));
            PDB.Add(new Persona("Arahabaki", "Ailment", 9, 35, new List<string>() { "Abysmal Surge", "Makarakarn", "Null Brainwash", "Spirit Drain", "Maeiga", "Defense Master" }));
            PDB.Add(new Persona("Thoth", "Nuclear", 4, 36, new List<string>() { "Freila", "Taunt", "Masukunda", "Megido", "Psy Wall", "Mafreila", "Growth 2" }));
            PDB.Add(new Persona("Kaiwan", "None", 17, 36, new List<string>() { "Psio", "Makajam", "Forget Boost", "Speed Master", "Makajamaon", "Mapsio", "Marakunda" }));
            PDB.Add(new Persona("Belphegor", "Ice", 16, 37, new List<string>() { "Bufula", "Dodge Fire", "Null Rage", "Ice Break", "Mabufula", "Concentrate" }));
            PDB.Add(new Persona("Anubis", "None", 20, 37, new List<string>() { "Hamaon", "Makouha", "Mudoon", "Null Fear", "Dekunda", "Resist Bless", "Eiga" }));
            PDB.Add(new Persona("Legion", "Psy", 0, 38, new List<string>() { "Negative Pile", "Rampage", "Life Drain", "Psio", "Tetra Break", "Null Dizzy" }));
            PDB.Add(new Persona("Unicorn", "Bless", 5, 39, new List<string>() { "Assault Dive", "Mahama", "Dekunda", "Samarecarm", "Swift Strike", "Kouga", "Hamaon" }));
            PDB.Add(new Persona("White Rider", "Curse", 7, 39, new List<string>() { "Oni Kagura", "Triple Down", "Evil Touch", "Snipe", "Maeiga", "Masukukaja", "Foul Breath", "Ailment Boost" }));
            PDB.Add(new Persona("Atropos", "Lightning", 10, 39, new List<string>() { "Mazionga", "Elec Break", "Fire Wall", "Mediarama", "Elec Boost", "Dodge Fire", "Concentrate" }));
            PDB.Add(new Persona("Hell Biker", "Fire", 13, 39, new List<string>() { "Agilao", "Mudoon", "Speed Master", "Fire Boost", "Tentafaroo", "Maragion", "Trigger Happy", "Mamudoon" }));
            PDB.Add(new Persona("Mithras", "Nuclear", 19, 39, new List<string>() { "Vicious Strike", "Mafreila", "Tentafaroo", "Tetra Break", "Nuke Break", "Freidyne" }));
            PDB.Add(new Persona("Kikuri-Hime", "Healing", 2, 40, new List<string>() { "Lullaby", "Energy Drop", "Marakukaja", "Mediarama", "Tetraja", "Divine Grace" }));
            PDB.Add(new Persona("Hariti", "Lightning", 4, 40, new List<string>() { "Zionga", "Energy Shower", "Mabaisudi", "Samarecarm", "Nocturnal Flash", "Mediarama", "Dizzy Boost", "Spirit Drain" }));
            PDB.Add(new Persona("Power", "Bless", 8, 41, new List<string>() { "Hamaon", "Sukukaja", "Swift Strike", "Makouga", "Diarama", "Masukukaja", "Null Curse" }));
            PDB.Add(new Persona("Red Rider", "Psy", 16, 41, new List<string>() { "Rising Slash", "Mapsio", "Psy Break", "Negative Pile", "Resist Confuse", "Pressing Stance", "Rage Boost" }));
            PDB.Add(new Persona("Ose", "Ailment", 0, 42, new List<string>() { "Oni Kagura", "Counterstrike", "Tempest Slash", "Matarukaja", "Heat Wave" }));
            PDB.Add(new Persona("Daisoujou", "Bless", 5, 42, new List<string>() { "Makouga", "Spirit Drain", "Bless Boost", "Diaharan", "Me Patra", "Null Rage" }));
            PDB.Add(new Persona("Kushinada", "Healing", 6, 42, new List<string>() { "Hysterical Slap", "Mabufula", "Null Sleep", "Wind Wall", "Amrita Shower" }));
            PDB.Add(new Persona("Kumbhanda", "Ailment", 9, 42, new List<string>() { "Hysterical Slap", "Wage War", "Stagnant Air", "Tempest Slash", "Dekaja", "Rage Boost", "Revolution" }));
            PDB.Add(new Persona("Hecatoncheires", "Psy", 12, 42, new List<string>() { "Swift Strike", "Tarukaja", "Regenerate 2", "Endure", "Foul Breath", "Fortified Moxy", "Charge" }));
            PDB.Add(new Persona("Yurlungur", "Lightning", 19, 42, new List<string>() { "Mazionga", "Brain Jack", "Megido", "Revolution", "Elec Break", "Tetra Break", "Elec Boost" }));
            PDB.Add(new Persona("Queen Mab", "None", 1, 43, new List<string>() { "Mazionga", "Makajamaon", "Wind Wall", "Matarunda", "Makara Break", "Agidyne" }));
            PDB.Add(new Persona("Pazuzu", "Curse", 15, 43, new List<string>() { "Maeiga", "Mudoon", "Tentafaroo", "Ambient Aid", "Evil Smile", "Bad Beat", "Eigaon" }));
            PDB.Add(new Persona("Ananta", "Nuclear", 17, 43, new List<string>() { "Mafreila", "Elec Wall", "Defense Master", "Abysmal Surge", "Growth 2", "Marakukaja", "Freidyne", "Nuke Boost" }));
            PDB.Add(new Persona("Okuninushi", "Psy", 4, 44, new List<string>() { "Tempest Slash", "Mapsio", "Matarukaja", "Psy Boost", "Psy Break", "Evade Nuke", "Heat Wave" }));
            PDB.Add(new Persona("Valkyrie", "Physical", 11, 44, new List<string>() { "Rising Slash", "Counterstrike", "Attack Master", "Deathbound", "Matarukaja", "Dodge Physical" }));
            PDB.Add(new Persona("Girimehkala", "Ailment", 18, 44, new List<string>() { "Swift Strike", "Mudoon", "Marakunda", "Foul Breath", "Wage War", "Repel Phys" }));
            PDB.Add(new Persona("Scathach", "Wind", 2, 45, new List<string>() { "Tempest Slash", "Magarula", "Sharp Student", "Maragion", "Matarukaja", "Attack Master", "Endure" }));
            PDB.Add(new Persona("Fortuna", "Wind", 10, 46, new List<string>() { "Magarula", "Masukukaja", "Tetraja", "Garudyne", "Touch n' Go", "Amrita Drop", "Evade Elec" }));
            PDB.Add(new Persona("Rangda", "Curse", 1, 48, new List<string>() { "Bloodbath", "Swift Strike", "Counterstrike", "Eigaon", "Matarunda", "Mudoon" }));
            PDB.Add(new Persona("Koumokuten", "Physical", 9, 49, new List<string>() { "Assault Dive", "Revolution", "Regenerate 2", "Attack Master", "Matarukaja", "Nuke Wall", "Enduring Soul", "Deadly Fury" }));
            PDB.Add(new Persona("Byakko", "Ice", 14, 49, new List<string>() { "Swift Strike", "Mabufula", "Counterstrike", "Ice Boost", "Evade Fire", "Null Rage", "Bufudyne" }));
            PDB.Add(new Persona("Horus", "None", 19, 49, new List<string>() { "Kougaon", "Diarama", "Megido", "Touch n' Go", "Masukukaja", "Hamaon", "Hama Boost" }));
            PDB.Add(new Persona("Sarasvati", "Healing", 2, 50, new List<string>() { "Tentafaroo", "Me Patra", "Mediarama", "Null Sleep", "Dekaja", "Matarunda", "Diaharan" }));
            PDB.Add(new Persona("Dakini", "Physical", 3, 50, new List<string>() { "Bad Beat", "Giant Slice", "Rising Slash", "High Counter", "Deathbound", "Rebellion", "Charge" }));
            PDB.Add(new Persona("Narcissus", "Ailment", 6, 50, new List<string>() { "Magarula", "Nocturnal Flash", "Energy Drop", "Growth 3", "Dizzy Boost", "Mediarama", "Ambient Aid" }));
            PDB.Add(new Persona("Barong", "Lightning", 4, 52, new List<string>() { "Ziodyne", "Wage War", "Elec Break", "Invigorate 2", "Null Elec", "Maziodyne" }));
            PDB.Add(new Persona("Norn", "None", 10, 52, new List<string>() { "Ziodyne", "Garudyne", "Nocturnal Flash", "Diaharan", "Amrita Drop", "Tetraja", "Samarecarm" }));
            PDB.Add(new Persona("Garuda", "Wind", 17, 52, new List<string>() { "Heat Wave", "Garudyne", "Amrita Shower", "Masukukaja", "Evade Elec", "Magarudyne", "Wind Amp" }));
            PDB.Add(new Persona("Pale Rider", "Curse", 13, 53, new List<string>() { "Brain Shake", "Eigaon", "Abysmal Surge", "Megidola", "Curse Boost", "Evade Bless", "Deathbound" }));
            PDB.Add(new Persona("Ganesha", "Ailment", 19, 53, new List<string>() { "Giant Slice", "Miracle Punch", "Rebellion", "Tetraja", "Endure", "Masukunda", "Charge" }));
            PDB.Add(new Persona("Skadi", "Ice", 2, 55, new List<string>() { "Mabufula", "Evil Touch", "Null Despair", "Ghastly Wail", "Bufudyne", "Spirit Drain", "Repel Ice" }));
            PDB.Add(new Persona("Cerberus", "Fire", 7, 55, new List<string>() { "Megaton Raid", "Agidyne", "Rebellion", "High Counter", "Regenerate 2", "Enduring Soul" }));
            PDB.Add(new Persona("Raja Naga", "Lightning", 14, 55, new List<string>() { "Ziodyne", "Tentafaroo", "Elec Break", "Shock Boost", "Makarakarn", "Maziodyne", "Evade Wind" }));
            PDB.Add(new Persona("Titania", "Nuclear", 3, 56, new List<string>() { "Freidyne", "Lullaby", "Makara Break", "Mafreidyne", "Nuke Amp", "Mediaharan" }));
            PDB.Add(new Persona("Parvati", "Psy", 6, 56, new List<string>() { "Psiodyne", "Hamaon", "Diarama", "Energy Shower", "Diaharan", "Mapsiodyne", "Hama Boost" }));
            PDB.Add(new Persona("Kurama Tengu", "Wind", 9, 56, new List<string>() { "Brain Buster", "Heat Wave", "Masukunda", "Garudyne", "Growth 3", "Magarudyne" }));
            PDB.Add(new Persona("Yatagarasu", "Fire", 19, 57, new List<string>() { "Agidyne", "Dekunda", "Makara Break", "Mediaharan", "Pressing Stance", "Wind Break", "Null Wind" }));
            PDB.Add(new Persona("Baphomet", "None", 15, 58, new List<string>() { "Agidyne", "Burn Boost", "Evade Fire", "Bufudyne", "Ziodyne", "Shock Boost", "Freeze Boost" }));
            PDB.Add(new Persona("Surt", "Fire", 1, 59, new List<string>() { "Megaton Raid", "Agidyne", "Fire Break", "Maragidyne", "High Counter", "Inferno" }));
            PDB.Add(new Persona("Black Rider", "Curse", 16, 59, new List<string>() { "Flash Bomb", "Maeigaon", "Mamudoon", "Ambient Aid", "Bloodbath", "Ghastly Wail", "Megidola" }));
            PDB.Add(new Persona("Melchizedek", "Bless", 8, 60, new List<string>() { "Megaton Raid", "Hamaon", "Hama Boost", "Revolution", "Mahamaon", "Amrita Drop", "God's Hand" }));
            PDB.Add(new Persona("Moloch", "Psy", 12, 60, new List<string>() { "Psiodyne", "Evil Smile", "Stagnant Air", "Agidyne", "Ghastly Wail", "Absorb Fire", "Nuke Amp" }));
            PDB.Add(new Persona("Lilith", "None", 18, 60, new List<string>() { "Mabufudyne", "Mudoon", "Makara Break", "Magarudyne", "Spirit Drain", "Mamudoon", "Maragidyne" }));
            PDB.Add(new Persona("Dionysus", "Psy", 0, 61, new List<string>() { "Heat Wave", "Psiodyne", "Abysmal Surge", "Thermopylae", "Ailment Boost", "Maragidyne", "Amrita Shower" }));
            PDB.Add(new Persona("King Frost", "Ice", 4, 61, new List<string>() { "Megaton Raid", "Bufudyne", "Ice Break", "Freeze Boost", "Auto-Mataru", "Null Despair", "Ice Amp" }));
            PDB.Add(new Persona("Chernobog", "Ailment", 13, 62, new List<string>() { "Bloodbath", "Deadly Fury", "Mudoon", "Stagnant Air", "Deathbound", "Fear Boost", "Myriad Slashes" }));
            PDB.Add(new Persona("Seiyu", "Ice", 14, 62, new List<string>() { "Bufudyne", "Diaharan", "Marakukaja", "Repel Nuke", "Mabufudyne", "Amrita Drop", "Makarakarn" }));
            PDB.Add(new Persona("Nebiros", "Curse", 15, 62, new List<string>() { "Eigaon", "Mamudoon", "Marin Karin", "Maeigaon", "Curse Amp", "Evade Bless", "Megidolaon" }));
            PDB.Add(new Persona("Forneus", "Psy", 5, 63, new List<string>() { "Psiodyne", "Marin Karin", "Masukunda", "Survival Trick", "Stagnant Air", "Mapsiodyne", "Evade Psy" }));
            PDB.Add(new Persona("Quetzalcoatl", "Wind", 19, 63, new List<string>() { "Memory Blow", "Magarula", "Growth 3", "Regenerate 3", "Magarudyne", "Wind Amp" }));
            PDB.Add(new Persona("Thor", "Lightning", 7, 64, new List<string>() { "Megaton Raid", "Ziodyne", "High Counter", "Elec Amp", "Maziodyne", "Heat Up", "Attack Master" }));
            PDB.Add(new Persona("Hanuman", "Physical", 17, 64, new List<string>() { "Tempest Slash", "Matarunda", "Revolution", "Deathbound", "Tetra Break", "Regenerate 3" }));
            PDB.Add(new Persona("Yamata-no-Orochi", "Ice", 20, 64, new List<string>() { "Deathbound", "Oni Kagura", "Mabufudyne", "Repel Fire", "Adverse Resolve", "Unshaken Will" }));
            PDB.Add(new Persona("Oberon", "Lightning", 4, 66, new List<string>() { "Heat Wave", "Ziodyne", "Brain Jack", "Matarukaja", "Maziodyne", "Psy Wall", "Samarecarm", "Elec Amp" }));
            PDB.Add(new Persona("Bishamonten", "Nuclear", 5, 67, new List<string>() { "Freidyne", "Diaharan", "Deadly Fury", "Mafreidyne", "Nuke Amp", "Tetrakarn", "God's Hand" }));
            PDB.Add(new Persona("Cu Chulainn", "None", 17, 67, new List<string>() { "Deadly Fury", "Oni Kagura", "Ice Wall", "Matarukaja", "Dekunda", "Charge", "Enduring Soul" }));
            PDB.Add(new Persona("Dominion", "Bless", 8, 68, new List<string>() { "Hamaon", "Kougaon", "Nocturnal Flash", "Makougaon", "Hama Boost", "Mahamaon", "Evade Curse" }));
            PDB.Add(new Persona("Belial", "Curse", 15, 68, new List<string>() { "Agidyne", "Mamudoon", "Matarunda", "Survival Trick", "Maragidyne", "Heat Up", "Myriad Slashes" }));
            PDB.Add(new Persona("Lakshmi", "Healing", 10, 69, new List<string>() { "Bufudyne", "Lullaby", "Diaharan", "Mediaharan", "Rainy Play", "Amrita Shower", "Life Aid" }));
            PDB.Add(new Persona("Siegfried", "Physical", 11, 69, new List<string>() { "Megaton Raid", "Masukukaja", "High Counter", "Charge", "Auto-Mataru", "Vorpal Blade" }));
            PDB.Add(new Persona("Mot", "Ailment", 13, 72, new List<string>() { "Maziodyne", "Megidola", "Matarukaja", "Concentrate", "Megidolaon", "Repel Elec" }));
            PDB.Add(new Persona("Cybele", "Healing", 2, 73, new List<string>() { "Makougaon", "Mediaharan", "Samarecarm", "Bless Amp", "Auto-Maraku", "Absorb Bless", "Salvation" }));
            PDB.Add(new Persona("Mara", "Fire", 16, 73, new List<string>() { "One-shot Kill", "Maragidyne", "Tetra Break", "Charge", "Maeigaon", "Heat Up", "Firm Stance" }));
            PDB.Add(new Persona("Abaddon", "Curse", 20, 74, new List<string>() { "Deathbound", "Spirit Drain", "Makarakarn", "Survival Trick", "Absorb Phys", "Gigantomachia" }));
            PDB.Add(new Persona("Baal", "Wind", 4, 75, new List<string>() { "Magarudyne", "Matarukaja", "Revolution", "Panta Rhei", "Tetraja", "Charge" }));
            PDB.Add(new Persona("Sandalphon", "Curse", 18, 75, new List<string>() { "Mahamaon", "Amrita Shower", "Samarecarm", "Angelic Grace", "Repel Curse", "Sword Dance" }));
            PDB.Add(new Persona("Futsunushi", "Physical", 1, 76, new List<string>() { "Myriad Slashes", "Matarukaja", "Ali Dance", "Charge", "Regenerate 3", "Apt Pupil", "Firm Stance", "Brave Blade" }));
            PDB.Add(new Persona("Kali", "Fire", 3, 77, new List<string>() { "Vorpal Blade", "Psiodyne", "Tentafaroo", "Evade Ice", "High Counter", "Mapsiodyne", "Absorb Nuke" }));
            PDB.Add(new Persona("Gabriel", "None", 14, 77, new List<string>() { "Mabufudyne", "Maziodyne", "Divine Judgement", "Ali Dance", "Evade Curse", "Touch n' Go", "Ice Amp", "Salvation" }));
            PDB.Add(new Persona("Raphael", "None", 6, 78, new List<string>() { "Sword Dance", "Charge", "Dekaja", "Heat Riser", "Growth 3", "Adverse Resolve", "Arms Master" }));
            PDB.Add(new Persona("Mother Harlot", "Ice", 3, 80, new List<string>() { "Mabufudyne", "Mamudoon", "Mudo Boost", "Ice Age", "Ice Amp", "Null Bless", "Debilitate" }));
            PDB.Add(new Persona("Zaou-Gongen", "Fire", 11, 80, new List<string>() { "God's Hand", "Maragidyne", "Abysmal Surge", "Evade Physical", "Enduring Soul", "Cripple", "Blazing Hell" }));
            PDB.Add(new Persona("Uriel", "None", 8, 81, new List<string>() { "Bloodbath", "Deathbound", "Myriad Slashes", "Repel Nuke", "Megidolaon", "Angelic Grace", "Spell Master" }));
            PDB.Add(new Persona("Odin", "Lightning", 4, 82, new List<string>() { "Myriad Slashes", "Thunder Reign", "Marakukaja", "Wild Thunder", "Concentrate", "Fast Heal", "Elec Amp" }));
            PDB.Add(new Persona("Attis", "Fire", 12, 82, new List<string>() { "Maragidyne", "Salvation", "Thermopylae", "Enduring Soul", "Samarecarm", "Absorb Curse", "Blazing Hell" }));
            PDB.Add(new Persona("Vishnu", "None", 0, 83, new List<string>() { "Magarudyne", "Megidolaon", "Ali Dance", "Vacuum Wave", "Charge", "Repel Fire", "Wind Amp", "Riot Gun" }));
            PDB.Add(new Persona("Beelzebub", "Curse", 15, 84, new List<string>() { "Maeigaon", "Mamudoon", "Evil Smile", "Curse Amp", "Concentrate", "Demonic Decree", "Repel Ice", "Megidolaon" }));
            PDB.Add(new Persona("Ishtar", "Healing", 6, 85, new List<string>() { "Mediaharan", "Samarecarm", "Absorb Wind", "Insta-Heal", "Maziodyne", "Spell Master", "Salvation" }));
            PDB.Add(new Persona("Mada", "Fire", 16, 85, new List<string>() { "Agidyne", "Burn Boost", "Fire Amp", "Unshaken Will", "Blazing Hell", "Amrita Shower", "Enduring Soul", "Spell Master" }));
            PDB.Add(new Persona("Satan", "Ice", 20, 92, new List<string>() { "Diamond Dust", "Ice Age", "Regenerate 3", "Black Viper", "Invigorate 3", "Fortify Spirit", "Concentrate", "Absorb Ice" }));

            /*PDB.Add(new Persona("Satanael", "None", 0, 95, new List<string>() { "Riot Gun", "Maeigaon", "Megidolaon", "Survival Trick", "Cosmic Flare", "Heat Riser", "Unshaken Will", "Victory Cry" }));
            PDB.Add(new Persona("Black Frost", "None", 0, 67, new List<string>() { "Miracle Punch", "One-shot Kill", "Mabufudyne", "Ice Amp", "Repel Fire", "Diamond Dust" }));
            PDB.Add(new Persona("Bugs", "None", 0, 49, new List<string>() { "Miracle Punch", "Psiodyne", "Masukunda", "Auto-Mataru", "Triple Down", "Evade Physical", "Fast Heal" }));
            PDB.Add(new Persona("Kohryu", "Psy", 5, 76, new List<string>() { "Mapsiodyne", "Psycho Force", "Mediaharan", "Life Aid", "Concentrate", "Psy Amp", "Spell Master" }));
            PDB.Add(new Persona("Chi You", "Psy", 6, 86, new List<string>() { "Gigantomachia", "Psycho Force", "Repel Phys", "Fortify Spirit", "Psycho Blast", "Absorb Psy", "Concentrate" }));
            PDB.Add(new Persona("Metatron", "Bless", 8, 89, new List<string>() { "Sword Dance", "Mahamaon", "Makougaon", "Megidolaon", "Hama Boost", "Concentrate", "Bless Amp", "Divine Judgement" }));
            PDB.Add(new Persona("Throne", "Bless", 8, 71, new List<string>() { "Mahamaon", "Hama Boost", "Invigorate 3", "Makougaon", "Bless Amp", "Evade Curse", "Auto-Maraku" }));
            PDB.Add(new Persona("Ongyo-Ki", "Physical", 9, 75, new List<string>() { "Myriad Slashes", "Makajamaon", "Pressing Stance", "Arms Master", "Regenerate 3", "Firm Stance", "Agneyastra" }));
            PDB.Add(new Persona("Vasuki", "Ailment", 12, 68, new List<string>() { "Triple Down", "Mahamaon", "Brain Jack", "Evade Wind", "Trigger Happy", "Brainwash Boost", "Makarakarn" }));
            PDB.Add(new Persona("Alice", "Curse", 13, 79, new List<string>() { "Mamudoon", "Dekunda", "Mudo Boost", "Megidolaon", "Concentrate", "Survival Trick" }));
            PDB.Add(new Persona("Ardha", "None", 14, 84, new List<string>() { "God's Hand", "Cosmic Flare", "Invigorate 3", "Agneyastra", "Auto-Masuku", "Fortified Moxy", "Salvation" }));
            PDB.Add(new Persona("Flauros", "Ailment", 15, 25, new List<string>() { "Dormin Rush", "Giant Slice", "Dekaja", "Dodge Physical", "Assault Dive", "Heat Up" }));
            PDB.Add(new Persona("Yoshitsune", "Physical", 16, 79, new List<string>() { "Brave Blade", "Ziodyne", "Charge", "Pressing Stance", "Fast Heal", "Elec Amp" }));
            PDB.Add(new Persona("Seth", "Fire", 16, 51, new List<string>() { "One-shot Kill", "Agidyne", "Masukukaja", "Cripple", "Fire Break", "Fortify Spirit" }));
            PDB.Add(new Persona("Lucifer", "None", 17, 93, new List<string>() { "Gigantomachia", "Blazing Hell", "Spell Master", "Heat Riser", "Fortified Moxy", "Insta-Heal", "Absorb Phys" }));
            PDB.Add(new Persona("Sraosha", "Bless", 17, 80, new List<string>() { "Kougaon", "Mahamaon", "Hama Boost", "Makougaon", "Angelic Grace", "Amrita Shower", "Debilitate" }));
            PDB.Add(new Persona("Neko Shogun", "None", 17, 30, new List<string>() { "Psio", "Diarama", "Masukukaja", "Invigorate 1", "Rat Fang", "Defense Master", "Fortified Moxy" }));
            PDB.Add(new Persona("Asura", "Nuclear", 19, 76, new List<string>() { "Atomic Flare", "Mahamaon", "Marakukaja", "Auto-Mataru", "Mafreidyne", "High Counter", "Unshaken Will" }));
            PDB.Add(new Persona("Michael", "None", 20, 87, new List<string>() { "Mabufudyne", "Divine Judgement", "Debilitate", "Sword Dance", "Mahamaon", "Megidolaon", "Cosmic Flare" }));
            PDB.Add(new Persona("Shiva", "Psy", 20, 82, new List<string>() { "Maziodyne", "Psycho Force", "Enduring Soul", "Riot Gun", "Megidolaon", "Auto-Mataru", "Psycho Blast" }));
            PDB.Add(new Persona("Trumpeter", "None", 20, 59, new List<string>() { "Brain Buster", "Mafreidyne", "Abysmal Surge", "Fortify Spirit", "Cripple", "Life Aid", "Debilitate" }));*/
        }

        static void FillMoves()
        {
            SDB.Add("Lunge", "Physical");
            SDB.Add("Cleave", "Physical");
            SDB.Add("Lucky Punch", "Physical");
            SDB.Add("Dream Needle", "Physical");
            SDB.Add("Miracle Punch", "Physical");
            SDB.Add("Terror Claw", "Physical");
            SDB.Add("Brain Shake", "Physical");
            SDB.Add("Giant Slice", "Physical");
            SDB.Add("Headbutt", "Physical");
            SDB.Add("Hysterical Slap", "Physical");
            SDB.Add("Double Fangs", "Physical");
            SDB.Add("Rat Fang", "Physical");
            SDB.Add("Skull Cracker", "Physical");
            SDB.Add("Sledgehammer", "Physical");
            SDB.Add("Negative Pile", "Physical");
            SDB.Add("Assault Dive", "Physical");
            SDB.Add("Rampage", "Physical");
            SDB.Add("Rising Slash", "Physical");
            SDB.Add("Vajra Blast", "Physical");
            SDB.Add("Memory Blow", "Physical");
            SDB.Add("Dormin Rush", "Physical");
            SDB.Add("Megaton Raid", "Physical");
            SDB.Add("Oni Kagura", "Physical");
            SDB.Add("Swift Strike", "Physical");
            SDB.Add("Tempest Slash", "Physical");
            SDB.Add("Deadly Fury", "Physical");
            SDB.Add("Vicious Strike", "Physical");
            SDB.Add("Bloodbath", "Physical");
            SDB.Add("Flash Bomb", "Physical");
            SDB.Add("Mind Slice", "Physical");
            SDB.Add("Heat Wave", "Physical");
            SDB.Add("Myriad Slashes", "Physical");
            SDB.Add("Bad Beat", "Physical");
            SDB.Add("Brain Buster", "Physical");
            SDB.Add("Sword Dance", "Physical");
            SDB.Add("Deathbound", "Physical");
            SDB.Add("Vorpal Blade", "Physical");
            SDB.Add("Agneyastra", "Physical");
            SDB.Add("Brave Blade", "Physical");
            SDB.Add("Gigantomachia", "Physical");
            SDB.Add("God's Hand", "Physical");
            SDB.Add("Snap", "Physical");
            SDB.Add("Triple Down", "Physical");
            SDB.Add("One-shot Kill", "Physical");
            SDB.Add("Riot Gun", "Physical");
            SDB.Add("Agi", "Fire");
            SDB.Add("Agilao", "Fire");
            SDB.Add("Maragi", "Fire");
            SDB.Add("Agidyne", "Fire");
            SDB.Add("Maragion", "Fire");
            SDB.Add("Maragidyne", "Fire");
            SDB.Add("Inferno", "Fire");
            SDB.Add("Blazing Hell", "Fire");
            SDB.Add("Bufu", "Ice");
            SDB.Add("Bufula", "Ice");
            SDB.Add("Mabufu", "Ice");
            SDB.Add("Bufudyne", "Ice");
            SDB.Add("Mabufula", "Ice");
            SDB.Add("Mabufudyne", "Ice");
            SDB.Add("Diamond Dust", "Ice");
            SDB.Add("Ice Age", "Ice");
            SDB.Add("Zio", "Lightning");
            SDB.Add("Zionga", "Lightning");
            SDB.Add("Mazio", "Lightning");
            SDB.Add("Ziodyne", "Lightning");
            SDB.Add("Mazionga", "Lightning");
            SDB.Add("Maziodyne", "Lightning");
            SDB.Add("Thunder Reign", "Lightning");
            SDB.Add("Wild Thunder", "Lightning");
            SDB.Add("Garu", "Wind");
            SDB.Add("Garula", "Wind");
            SDB.Add("Magaru", "Wind");
            SDB.Add("Garudyne", "Wind");
            SDB.Add("Magarula", "Wind");
            SDB.Add("Magarudyne", "Wind");
            SDB.Add("Panta Rhei", "Wind");
            SDB.Add("Vacuum Wave", "Wind");
            SDB.Add("Psi", "Psy");
            SDB.Add("Psio", "Psy");
            SDB.Add("Mapsi", "Psy");
            SDB.Add("Psiodyne", "Psy");
            SDB.Add("Mapsio", "Psy");
            SDB.Add("Mapsiodyne", "Psy");
            SDB.Add("Psycho Force", "Psy");
            SDB.Add("Psycho Blast", "Psy");
            SDB.Add("Frei", "Nuclear");
            SDB.Add("Freila", "Nuclear");
            SDB.Add("Mafrei", "Nuclear");
            SDB.Add("Freidyne", "Nuclear");
            SDB.Add("Mafreila", "Nuclear");
            SDB.Add("Mafreidyne", "Nuclear");
            SDB.Add("Atomic Flare", "Nuclear");
            SDB.Add("Cosmic Flare", "Nuclear");
            SDB.Add("Kouha", "Bless");
            SDB.Add("Hama", "Bless");
            SDB.Add("Kouga", "Bless");
            SDB.Add("Makouha", "Bless");
            SDB.Add("Kougaon", "Bless");
            SDB.Add("Hamaon", "Bless");
            SDB.Add("Makouga", "Bless");
            SDB.Add("Mahama", "Bless");
            SDB.Add("Makougaon", "Bless");
            SDB.Add("Mahamaon", "Bless");
            SDB.Add("Divine Judgement", "Bless");
            SDB.Add("Eiha", "Curse");
            SDB.Add("Mudo", "Curse");
            SDB.Add("Eiga", "Curse");
            SDB.Add("Maeiha", "Curse");
            SDB.Add("Eigaon", "Curse");
            SDB.Add("Mudoon", "Curse");
            SDB.Add("Maeiga", "Curse");
            SDB.Add("Mamudo", "Curse");
            SDB.Add("Maeigaon", "Curse");
            SDB.Add("Mamudoon", "Curse");
            SDB.Add("Demonic Decree", "Curse");
            SDB.Add("Dazzler", "Ailment");
            SDB.Add("Dormina", "Ailment");
            SDB.Add("Evil Touch", "Ailment");
            SDB.Add("Makajam", "Ailment");
            SDB.Add("Marin Karin", "Ailment");
            SDB.Add("Ominous Words", "Ailment");
            SDB.Add("Pulinpa", "Ailment");
            SDB.Add("Taunt", "Ailment");
            SDB.Add("Abysmal Surge", "Ailment");
            SDB.Add("Brain Jack", "Ailment");
            SDB.Add("Evil Smile", "Ailment");
            SDB.Add("Lullaby", "Ailment");
            SDB.Add("Makajamaon", "Ailment");
            SDB.Add("Nocturnal Flash", "Ailment");
            SDB.Add("Tentafaroo", "Ailment");
            SDB.Add("Wage War", "Ailment");
            SDB.Add("Dia", "Healing");
            SDB.Add("Baisudi", "Healing");
            SDB.Add("Energy Drop", "Healing");
            SDB.Add("Patra", "Healing");
            SDB.Add("Amrita Drop", "Healing");
            SDB.Add("Diarama", "Healing");
            SDB.Add("Media", "Healing");
            SDB.Add("Energy Shower", "Healing");
            SDB.Add("Mabaisudi", "Healing");
            SDB.Add("Me Patra", "Healing");
            SDB.Add("Recarm", "Healing");
            SDB.Add("Amrita Shower", "Healing");
            SDB.Add("Mediarama", "Healing");
            SDB.Add("Diaharan", "Healing");
            SDB.Add("Samarecarm", "Healing");
            SDB.Add("Mediaharan", "Healing");
            SDB.Add("Salvation", "Healing");
            SDB.Add("Life Drain", "None");
            SDB.Add("Spirit Drain", "None");
            SDB.Add("Stagnant Air", "None");
            SDB.Add("Foul Breath", "None");
            SDB.Add("Megido", "None");
            SDB.Add("Megidola", "None");
            SDB.Add("Ghastly Wail", "None");
            SDB.Add("Megidolaon", "None");
            SDB.Add("Black Viper", "None");
            SDB.Add("Rebellion", "None");
            SDB.Add("Revolution", "None");
            SDB.Add("Rakukaja", "None");
            SDB.Add("Rakunda", "None");
            SDB.Add("Sukukaja", "None");
            SDB.Add("Sukunda", "None");
            SDB.Add("Tarukaja", "None");
            SDB.Add("Tarunda", "None");
            SDB.Add("Dekaja", "None");
            SDB.Add("Dekunda", "None");
            SDB.Add("Charge", "None");
            SDB.Add("Concentrate", "None");
            SDB.Add("Elec Break", "None");
            SDB.Add("Fire Break", "None");
            SDB.Add("Ice Break", "None");
            SDB.Add("Nuke Break", "None");
            SDB.Add("Psy Break", "None");
            SDB.Add("Wind Break", "None");
            SDB.Add("Elec Wall", "None");
            SDB.Add("Fire Wall", "None");
            SDB.Add("Ice Wall", "None");
            SDB.Add("Makara Break", "None");
            SDB.Add("Nuke Wall", "None");
            SDB.Add("Psy Wall", "None");
            SDB.Add("Tetra Break", "None");
            SDB.Add("Wind Wall", "None");
            SDB.Add("Marakukaja", "None");
            SDB.Add("Marakunda", "None");
            SDB.Add("Masukukaja", "None");
            SDB.Add("Masukunda", "None");
            SDB.Add("Matarukaja", "None");
            SDB.Add("Matarunda", "None");
            SDB.Add("Tetraja", "None");
            SDB.Add("Debilitate", "None");
            SDB.Add("Heat Riser", "None");
            SDB.Add("Thermopylae", "None");
            SDB.Add("Makarakarn", "None");
            SDB.Add("Tetrakarn", "None");
            SDB.Add("Absorb Bless", "None");
            SDB.Add("Absorb Curse", "None");
            SDB.Add("Absorb Elec", "None");
            SDB.Add("Absorb Fire", "None");
            SDB.Add("Absorb Ice", "None");
            SDB.Add("Absorb Nuke", "None");
            SDB.Add("Absorb Phys", "None");
            SDB.Add("Absorb Psy", "None");
            SDB.Add("Absorb Wind", "None");
            SDB.Add("Adverse Resolve", "None");
            SDB.Add("Ailment Boost", "None");
            SDB.Add("Ali Dance", "None");
            SDB.Add("Almighty Boost", "None");
            SDB.Add("Ambient Aid", "None");
            SDB.Add("Angelic Grace", "None");
            SDB.Add("Apt Pupil", "None");
            SDB.Add("Arms Master", "None");
            SDB.Add("Attack Master", "None");
            SDB.Add("Auto-Maraku", "None");
            SDB.Add("Auto-Masuku", "None");
            SDB.Add("Auto-Mataru", "None");
            SDB.Add("Bless Amp", "None");
            SDB.Add("Bless Boost", "None");
            SDB.Add("Brainwash Boost", "None");
            SDB.Add("Burn Boost", "None");
            SDB.Add("Confuse Boost", "None");
            SDB.Add("Counter", "None");
            SDB.Add("Counterstrike", "None");
            SDB.Add("Cripple", "None");
            SDB.Add("Curse Amp", "None");
            SDB.Add("Curse Boost", "None");
            SDB.Add("Defense Master", "None");
            SDB.Add("Despair Boost", "None");
            SDB.Add("Divine Grace", "None");
            SDB.Add("Dizzy Boost", "None");
            SDB.Add("Dodge Bless", "None");
            SDB.Add("Dodge Curse", "None");
            SDB.Add("Dodge Elec", "None");
            SDB.Add("Dodge Fire", "None");
            SDB.Add("Dodge Ice", "None");
            SDB.Add("Dodge Nuke", "None");
            SDB.Add("Dodge Physical", "None");
            SDB.Add("Dodge Psy", "None");
            SDB.Add("Dodge Wind", "None");
            SDB.Add("Elec Amp", "None");
            SDB.Add("Elec Boost", "None");
            SDB.Add("Endure", "None");
            SDB.Add("Enduring Soul", "None");
            SDB.Add("Evade Bless", "None");
            SDB.Add("Evade Curse", "None");
            SDB.Add("Evade Elec", "None");
            SDB.Add("Evade Fire", "None");
            SDB.Add("Evade Ice", "None");
            SDB.Add("Evade Nuke", "None");
            SDB.Add("Evade Physical", "None");
            SDB.Add("Evade Psy", "None");
            SDB.Add("Evade Wind", "None");
            SDB.Add("Fast Heal", "None");
            SDB.Add("Fear Boost", "None");
            SDB.Add("Fire Amp", "None");
            SDB.Add("Fire Boost", "None");
            SDB.Add("Firm Stance", "None");
            SDB.Add("Forget Boost", "None");
            SDB.Add("Fortified Moxy", "None");
            SDB.Add("Fortify Spirit", "None");
            SDB.Add("Freeze Boost", "None");
            SDB.Add("Growth 1", "None");
            SDB.Add("Growth 2", "None");
            SDB.Add("Growth 3", "None");
            SDB.Add("Hama Boost", "None");
            SDB.Add("Heat Up", "None");
            SDB.Add("High Counter", "None");
            SDB.Add("Ice Amp", "None");
            SDB.Add("Ice Boost", "None");
            SDB.Add("Insta-Heal", "None");
            SDB.Add("Invigorate 1", "None");
            SDB.Add("Invigorate 2", "None");
            SDB.Add("Invigorate 3", "None");
            SDB.Add("Life Aid", "None");
            SDB.Add("Mudo Boost", "None");
            SDB.Add("Nuke Amp", "None");
            SDB.Add("Nuke Boost", "None");
            SDB.Add("Null Bless", "None");
            SDB.Add("Null Brainwash", "None");
            SDB.Add("Null Confuse", "None");
            SDB.Add("Null Curse", "None");
            SDB.Add("Null Despair", "None");
            SDB.Add("Null Dizzy", "None");
            SDB.Add("Null Elec", "None");
            SDB.Add("Null Fear", "None");
            SDB.Add("Null Fire", "None");
            SDB.Add("Null Forget", "None");
            SDB.Add("Null Ice", "None");
            SDB.Add("Null Nuke", "None");
            SDB.Add("Null Phys", "None");
            SDB.Add("Null Psy", "None");
            SDB.Add("Null Rage", "None");
            SDB.Add("Null Sleep", "None");
            SDB.Add("Null Wind", "None");
            SDB.Add("Pressing Stance", "None");
            SDB.Add("Psy Amp", "None");
            SDB.Add("Psy Boost", "None");
            SDB.Add("Rage Boost", "None");
            SDB.Add("Rainy Play", "None");
            SDB.Add("Regenerate 1", "None");
            SDB.Add("Regenerate 2", "None");
            SDB.Add("Regenerate 3", "None");
            SDB.Add("Repel Bless", "None");
            SDB.Add("Repel Curse", "None");
            SDB.Add("Repel Elec", "None");
            SDB.Add("Repel Fire", "None");
            SDB.Add("Repel Ice", "None");
            SDB.Add("Repel Nuke", "None");
            SDB.Add("Repel Phys", "None");
            SDB.Add("Repel Psy", "None");
            SDB.Add("Repel Wind", "None");
            SDB.Add("Resist Bless", "None");
            SDB.Add("Resist Brainwash", "None");
            SDB.Add("Resist Confuse", "None");
            SDB.Add("Resist Curse", "None");
            SDB.Add("Resist Despair", "None");
            SDB.Add("Resist Dizzy", "None");
            SDB.Add("Resist Elec", "None");
            SDB.Add("Resist Fear", "None");
            SDB.Add("Resist Fire", "None");
            SDB.Add("Resist Forget", "None");
            SDB.Add("Resist Ice", "None");
            SDB.Add("Resist Nuke", "None");
            SDB.Add("Resist Phys", "None");
            SDB.Add("Resist Psy", "None");
            SDB.Add("Resist Rage", "None");
            SDB.Add("Resist Sleep", "None");
            SDB.Add("Resist Wind", "None");
            SDB.Add("Sharp Student", "None");
            SDB.Add("Shock Boost", "None");
            SDB.Add("Sleep Boost", "None");
            SDB.Add("Snipe", "None");
            SDB.Add("Speed Master", "None");
            SDB.Add("Spell Master", "None");
            SDB.Add("Survival Trick", "None");
            SDB.Add("Touch n' Go", "None");
            SDB.Add("Trigger Happy", "None");
            SDB.Add("Unshaken Will", "None");
            SDB.Add("Victory Cry", "None");
            SDB.Add("Wind Amp", "None");
            SDB.Add("Wind Boost", "None");
        }

        static void FillArcana()
        {
            arcanaDict.Add(0, "Fool");
            arcanaDict.Add(1, "Magician");
            arcanaDict.Add(2, "Priestess");
            arcanaDict.Add(3, "Empress");
            arcanaDict.Add(4, "Emperor");
            arcanaDict.Add(5, "Hierophant");
            arcanaDict.Add(6, "Lovers");
            arcanaDict.Add(7, "Chariot");
            arcanaDict.Add(8, "Justice");
            arcanaDict.Add(9, "Hermit");
            arcanaDict.Add(10, "Fortune");
            arcanaDict.Add(11, "Strength");
            arcanaDict.Add(12, "Hanged Man");
            arcanaDict.Add(13, "Death");
            arcanaDict.Add(14, "Temperance");
            arcanaDict.Add(15, "Devil");
            arcanaDict.Add(16, "Tower");
            arcanaDict.Add(17, "Star");
            arcanaDict.Add(18, "Moon");
            arcanaDict.Add(19, "Sun");
            arcanaDict.Add(20, "Judgement");
        }
    }

    class Persona
    {
        public string name;
        public List<string> restrictions = new List<string>();
        public int arcana;
        public int level;
        public List<string> moveset;

        public Persona(string n, string t, int a, int l, List<string> m)
        {
            name = n;
            arcana = a;
            level = l;
            moveset = m;
            switch (t)
            {
                case "Physical":
                    restrictions.Add("Fire");
                    restrictions.Add("Ice");
                    restrictions.Add("Lightning");
                    restrictions.Add("Wind");
                    restrictions.Add("Psy");
                    restrictions.Add("Nuclear");
                    restrictions.Add("Bless");
                    restrictions.Add("Curse");
                    break;
                case "Fire":
                    restrictions.Add("Ice");
                    break;
                case "Ice":
                    restrictions.Add("Fire");
                    break;
                case "Lightning":
                    restrictions.Add("Wind");
                    break;
                case "Wind":
                    restrictions.Add("Lightning");
                    break;
                case "Psy":
                    restrictions.Add("Nuclear");
                    break;
                case "Nuclear":
                    restrictions.Add("Psy");
                    break;
                case "Bless":
                    restrictions.Add("Physical");
                    restrictions.Add("Curse");
                    restrictions.Add("Ailment");
                    break;
                case "Curse":
                    restrictions.Add("Physical");
                    restrictions.Add("Bless");
                    restrictions.Add("Healing");
                    break;
                case "Ailment":
                    restrictions.Add("Bless");
                    restrictions.Add("Healing");
                    break;
                case "Healing":
                    restrictions.Add("Physical");
                    restrictions.Add("Curse");
                    break;
            }
        }
    }
}
