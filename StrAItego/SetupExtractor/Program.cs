using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StrAItego.Game;

namespace StradosExtractor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args) {
            //ExtractAllSetups();
            //ExtractSetupResults();
            //ExtractFullGames();
            //ExtractStatesFromGames();
            //CompressBigFile();
            //ShuffleCompressedFile();
            //ExtractSetupsDuringPlayFromGames();
            //CompressBigSDPFile();
            //ShuffleCompressedSDPFile();
            //ReplaceSetupCharsWithBytes();
            //ExtractSDPBaseAccuracyFromGames();
            //ExtractFullGamesWithResultsNoTies();
            //ExtractStatesFromGamesWithResult();
            //CompressBigFile();
            ShuffleCompressedFile();
        }

        static void ReplaceSetupCharsWithBytes() {
            using (BinaryWriter sw = new BinaryWriter(File.Open("ShuffledSDPDatabase2", FileMode.Create))) {
                using (BinaryReader sr = new BinaryReader(File.Open("ShuffledSDPDatabase", FileMode.Open))) {
                    byte[][] allbytes = new byte[17123744][];

                    // Read all bytes
                    for (int i = 0; i < allbytes.Length; i++) {
                        allbytes[i] = sr.ReadBytes(101);
                    }
                    Console.WriteLine("Finished reading");

                    // Write all bytes
                    for (int i = 0; i < allbytes.Length; i++) {
                        sw.Write(allbytes[i].Take(60).ToArray());
                        for (int j = 61; j < 101; j++) {
                            if (allbytes[i][j] == 0x4D) //Marshal
                                allbytes[i][j] = 0x0A;
                            else if (allbytes[i][j] == 0x42) //Bomb
                                allbytes[i][j] = 0x0B;
                            else if (allbytes[i][j] == 0x46) //Flag
                                allbytes[i][j] = 0x00;
                            else
                                allbytes[i][j] = (byte)(allbytes[i][j] - 0x30);
                        }
                        sw.Write(allbytes[i].Skip(61).ToArray());
                    }

                    Console.WriteLine("Finished writing");
                    sw.Flush();
                }
            }
            Console.ReadLine();
        }

        static void ShuffleCompressedSDPFile() {
            using (BinaryWriter sw = new BinaryWriter(File.Open("ShuffledSDPDatabase", FileMode.Create))) {
                using (BinaryReader sr = new BinaryReader(File.Open("CompressedSDPDatabase", FileMode.Open))) {
                    byte[][] allbytes = new byte[17123744][];

                    // Read all bytes
                    for (int i = 0; i < allbytes.Length; i++) {
                        allbytes[i] = sr.ReadBytes(101);
                    }
                    Console.WriteLine("Finished reading");

                    // Shuffle all bytes
                    Random rng = new Random();
                    int n = allbytes.Length;
                    while (n > 1) {
                        int k = rng.Next(n--);
                        byte[] temp = allbytes[n];
                        allbytes[n] = allbytes[k];
                        allbytes[k] = temp;
                    }

                    Console.WriteLine("Finished shuffling");

                    // Write all bytes
                    for (int i = 0; i < allbytes.Length; i++) {
                        sw.Write(allbytes[i]);
                    }

                    Console.WriteLine("Finished writing");
                    sw.Flush();
                }
            }
            Console.ReadLine();
        }

        static void ShuffleCompressedFile() {
            using (BinaryWriter sw = new BinaryWriter(File.Open("SWR", FileMode.Create))) {
                using (BinaryReader sr = new BinaryReader(File.Open("CompressedSWR", FileMode.Open))) {
                    byte[][] allbytes = new byte[17123744][];

                    // Read all bytes
                    for (int i = 0; i < allbytes.Length; i++) {
                        allbytes[i] = sr.ReadBytes(416);
                    }

                    Console.WriteLine("Finished reading");

                    // Shuffle all bytes
                    Random rng = new Random();
                    int n = allbytes.Length;
                    while (n > 1) {
                        int k = rng.Next(n--);
                        byte[] temp = allbytes[n];
                        allbytes[n] = allbytes[k];
                        allbytes[k] = temp;
                    }

                    Console.WriteLine("Finished shuffling");

                    // Write all bytes
                    for (int i = 0; i < allbytes.Length; i++) {
                        sw.Write(allbytes[i]);
                    }

                    Console.WriteLine("Finished writing");
                    sw.Flush();
                }
            }
            Console.ReadLine();
        }

        static void CompressBigSDPFile() {
            using (BinaryWriter sw = new BinaryWriter(File.Open("CompressedSDPDatabase", FileMode.Create))) {
                using (StreamReader sr = new StreamReader("DatabaseSetupsDuringPlay.txt")) {
                    string line = sr.ReadLine();
                    int c = 0;
                    while (line != null) {
                        string[] splitline = line.Split(';');
                        byte[] bytes = new byte[60];
                        for (int i = 0; i < bytes.Length; i++) {
                            int data = 0;
                            for (int j = 0; j < 8; j++) {
                                int n = splitline[0][i * 8 + j] == '1' ? 1 : 0;
                                data += n << (7 - j);
                            }
                            bytes[i] = Convert.ToByte(data);
                        }

                        sw.Write(bytes);
                        sw.Write(splitline[1]);

                        line = sr.ReadLine();
                        c++;
                        if (c % 10000 == 0)
                            Console.WriteLine($"Finished {c} lines...");
                    }
                    sw.Flush();
                    Console.WriteLine($"Finished compressing {c} states!");
                }
            }
            Console.ReadLine();
        }

        static void CompressBigFile() {
            using (BinaryWriter sw = new BinaryWriter(File.Open("CompressedSWR", FileMode.Create))) {
                using (StreamReader sr = new StreamReader("StatesWithResults.txt")) {
                    string line = sr.ReadLine();
                    int c = 0;
                    while (line != null) {
                        string[] splitline = line.Split(';');
                        short moveid = Convert.ToInt16(splitline[1]);
                        byte[] bytes = new byte[414];
                        for (int i = 0; i < bytes.Length; i++) {
                            int data = 0;
                            for (int j = 0; j < 8; j++) {
                                int n = splitline[0][i * 8 + j] == '1' ? 1 : 0;
                                data += n << (7 - j);
                            }
                            bytes[i] = Convert.ToByte(data);
                        }

                        sw.Write(bytes);
                        sw.Write(moveid);

                        line = sr.ReadLine();
                        c++;
                        if (c % 10000 == 0)
                            Console.WriteLine($"Finished {c} lines...");
                    }
                    sw.Flush();
                    Console.WriteLine($"Finished compressing {c} states!");
                }
            }
            Console.ReadLine();
        }

        static void ExtractSDPBaseAccuracyFromGames() {
            int easyGuesses = 0;
            List<int> issues = new List<int>();
            using (StreamReader sr = new StreamReader("Database.txt")) {
                string line = sr.ReadLine();
                int c = 0;
                float[] binboard = new float[480];   // 480 for known info + 40 for actual labels
                while (line != null) {
                    try {
                        // Every line is a game
                        string[] splitline = line.Split(';');
                        Rank[] p1setup = new Rank[40];
                        for (int i = 0; i < 40; i++)
                            p1setup[i] = CharToRank(splitline[0][i]);
                        Rank[] p2setup = new Rank[40];
                        for (int i = 0; i < 40; i++)
                            p2setup[i] = CharToRank(splitline[1][i]);

                        Board b = new Board();
                        b.TakeSetup(p2setup);
                        b.Invert();
                        b.TakeSetup(p1setup);

                        //if(c == 3206) {
                        //    string p1 = Deconvert(splitline[0], false);
                        //    string p2 = Deconvert(splitline[1], true);
                        //    string whole = $"{p1}AA__AA__AAAA__AA__AA{p2}";
                        //    Console.WriteLine(whole);
                        //    Clipboard.SetText(whole);
                        //}


                        bool inverted = false;
                        for (int i = 2; i < splitline.Length; i++) {
                            // Convert board to binary
                            binboard = b.PiecesToBinary(Team.Blue, binboard);
                            // Count total number of easy guesses
                            for (int j = 0; j < 40; j++) {
                                float bitsactive = 0f;
                                for(int k = 0; k < 12; k++) {
                                    bitsactive += binboard[j * 12 + k];
                                }
                                if (bitsactive > 1f)
                                    easyGuesses++;
                            }
                            Rank[] setup = inverted ? p1setup : p2setup;
                            // Get move from splitline[i]
                            int moveid = int.Parse(splitline[i]);
                            // Make move
                            (Square from, Square to) = Board.GetMoveAtIndex(moveid);
                            Move move = new Move(b[from], from, to, b[to], moveid);
                            if (move.Attacker == null) {
                                issues.Add(c);
                                issues.Add(i);
                                throw new ArgumentException($"Illegal move! at game {c + 1} at move {i - 1}");
                            }
                            b.MakeMove(move);
                            //if (c == 3206) {   //3206 7064 20109 21922
                            //    if (inverted)
                            //        move = move.GetInvertedMove();
                            //    Console.WriteLine(move);
                            //}
                            b.Invert();
                            inverted = !inverted;
                        }
                    }
                    catch {
                        Console.WriteLine($"Issue(s) on line {c}");
                    }

                    c++;
                    if (c % 100 == 0)
                        Console.WriteLine("Finished processing " + c + " games.");
                    
                    line = sr.ReadLine();
                }
            }
            Console.WriteLine($"Found {easyGuesses}, {easyGuesses / (17123744 * 40f) * 100f}% of total");
            foreach (int i in issues)
                Console.WriteLine(i);
            Console.ReadLine();
        }

        static void ExtractSetupsDuringPlayFromGames() {
            List<int> issues = new List<int>();
            using (StreamWriter sw = new StreamWriter("DatabaseSetupsDuringPlay.txt")) {
                using (StreamReader sr = new StreamReader("Database.txt")) {
                    string line = sr.ReadLine();
                    int c = 0;
                    float[] binboard = new float[480];   // 480 for known info + 40 for actual labels
                    while (line != null) {
                        try {
                            // Every line is a game
                            string[] splitline = line.Split(';');
                            Rank[] p1setup = new Rank[40];
                            for (int i = 0; i < 40; i++)
                                p1setup[i] = CharToRank(splitline[0][i]);
                            Rank[] p2setup = new Rank[40];
                            for (int i = 0; i < 40; i++)
                                p2setup[i] = CharToRank(splitline[1][i]);

                            Board b = new Board();
                            b.TakeSetup(p2setup);
                            b.Invert();
                            b.TakeSetup(p1setup);

                            //if(c == 3206) {
                            //    string p1 = Deconvert(splitline[0], false);
                            //    string p2 = Deconvert(splitline[1], true);
                            //    string whole = $"{p1}AA__AA__AAAA__AA__AA{p2}";
                            //    Console.WriteLine(whole);
                            //    Clipboard.SetText(whole);
                            //}

                            StringBuilder sb = new StringBuilder(521);
                            bool inverted = false;
                            for (int i = 2; i < splitline.Length; i++) {
                                // Convert board to binary
                                binboard = b.PiecesToBinary(Team.Blue, binboard);
                                // Write binary to file
                                sb.Clear();
                                foreach (float f in binboard)
                                    sb.Append(f == 0f ? '0' : '1');
                                sb.Append(';');
                                Rank[] setup = inverted ? p1setup : p2setup;
                                foreach (Rank r in setup)
                                    sb.Append(RankToChar(r));
                                sw.WriteLine(sb);
                                // Get move from splitline[i]
                                int moveid = int.Parse(splitline[i]);
                                // Make move
                                (Square from, Square to) = Board.GetMoveAtIndex(moveid);
                                Move move = new Move(b[from], from, to, b[to], moveid);
                                if (move.Attacker == null) {
                                    issues.Add(c);
                                    issues.Add(i);
                                    throw new ArgumentException($"Illegal move! at game {c + 1} at move {i - 1}");
                                }
                                b.MakeMove(move);
                                //if (c == 3206) {   //3206 7064 20109 21922
                                //    if (inverted)
                                //        move = move.GetInvertedMove();
                                //    Console.WriteLine(move);
                                //}
                                b.Invert();
                                inverted = !inverted;
                            }
                        }
                        catch {
                            Console.WriteLine($"Issue(s) on line {c}");
                        }
                        sw.Flush();
                        c++;
                        if (c % 100 == 0)
                            Console.WriteLine("Finished processing " + c + " games.");

                        line = sr.ReadLine();
                    }
                }
            }
            foreach (int i in issues)
                Console.WriteLine(i);
            Console.ReadLine();
        }

        static void ExtractStatesFromGamesWithResult() {
            List<int> issues = new List<int>();
            using (StreamWriter sw = new StreamWriter("StatesWithResults.txt")) {
                using (StreamReader sr = new StreamReader("DBWRNT.txt")) {
                    string line = sr.ReadLine();
                    int c = 0;
                    float[] binboard = new float[2208];
                    while (line != null) {
                        try {
                            // Every line is a game
                            string[] splitline = line.Split(';');
                            Rank[] p1setup = new Rank[40];
                            for (int i = 0; i < 40; i++)
                                p1setup[i] = CharToRank(splitline[1][i]);
                            Rank[] p2setup = new Rank[40];
                            for (int i = 0; i < 40; i++)
                                p2setup[i] = CharToRank(splitline[2][i]);

                            Board b = new Board();
                            b.TakeSetup(p2setup);
                            b.Invert();
                            b.TakeSetup(p1setup);

                            //if(c == 3206) {
                            //    string p1 = Deconvert(splitline[0], false);
                            //    string p2 = Deconvert(splitline[1], true);
                            //    string whole = $"{p1}AA__AA__AAAA__AA__AA{p2}";
                            //    Console.WriteLine(whole);
                            //    Clipboard.SetText(whole);
                            //}

                            StringBuilder sb = new StringBuilder(2208);
                            bool inverted = false;
                            for (int i = 3; i < splitline.Length; i++) {
                                // Convert board to binary
                                float[] binary = b.ToBinary(Team.Red);
                                // Write binary to file
                                sb.Clear();
                                foreach (float f in binary)
                                    sb.Append(f == 0f ? '0' : '1');
                                sw.Write(sb);
                                // Add move to file from splitline[i]
                                int moveid = int.Parse(splitline[i]);
                                sw.WriteLine($";{(splitline[0] == "0" ? inverted ? "1" : "0" : inverted ? "0" : "1")}");
                                // Make move
                                (Square from, Square to) = Board.GetMoveAtIndex(moveid);
                                Move move = new Move(b[from], from, to, b[to], moveid);
                                if (move.Attacker == null) {
                                    issues.Add(c);
                                    issues.Add(i);
                                    throw new ArgumentException($"Illegal move! at game {c + 1} at move {i - 1}");
                                }
                                b.MakeMove(move);
                                //if (c == 3206) {   //3206 7064 20109 21922
                                //    if (inverted)
                                //        move = move.GetInvertedMove();
                                //    Console.WriteLine(move);
                                //}
                                b.Invert();
                                inverted = !inverted;
                            }
                        }
                        catch {
                            Console.WriteLine($"Issue(s) on line {c}");
                        }
                        sw.Flush();
                        c++;
                        if (c % 100 == 0)
                            Console.WriteLine("Finished processing " + c + " games.");

                        line = sr.ReadLine();
                    }
                }
            }
            foreach (int i in issues)
                Console.WriteLine(i);
            Console.ReadLine();
        }

        static void ExtractStatesFromGames() {
            List<int> issues = new List<int>();
            using (StreamWriter sw = new StreamWriter("result.txt")) {
                using (StreamReader sr = new StreamReader("Database.txt")) {
                    string line = sr.ReadLine();
                    int c = 0;
                    float[] binboard = new float[2208];
                    while (line != null) {
                        try {
                            // Every line is a game
                            string[] splitline = line.Split(';');
                            Rank[] p1setup = new Rank[40];
                            for (int i = 0; i < 40; i++)
                                p1setup[i] = CharToRank(splitline[0][i]);
                            Rank[] p2setup = new Rank[40];
                            for (int i = 0; i < 40; i++)
                                p2setup[i] = CharToRank(splitline[1][i]);

                            Board b = new Board();
                            b.TakeSetup(p2setup);
                            b.Invert();
                            b.TakeSetup(p1setup);

                            //if(c == 3206) {
                            //    string p1 = Deconvert(splitline[0], false);
                            //    string p2 = Deconvert(splitline[1], true);
                            //    string whole = $"{p1}AA__AA__AAAA__AA__AA{p2}";
                            //    Console.WriteLine(whole);
                            //    Clipboard.SetText(whole);
                            //}

                            StringBuilder sb = new StringBuilder(2208);
                            bool inverted = false;
                            for (int i = 2; i < splitline.Length; i++) {
                                // Convert board to binary
                                float[] binary = b.ToBinary(Team.Red);
                                // Write binary to file
                                sb.Clear();
                                foreach (float f in binary)
                                    sb.Append(f == 0f ? '0' : '1');
                                sw.Write(sb);
                                // Add move to file from splitline[i]
                                int moveid = int.Parse(splitline[i]);
                                sw.WriteLine($";{moveid}");
                                // Make move
                                (Square from, Square to) = Board.GetMoveAtIndex(moveid);
                                Move move = new Move(b[from], from, to, b[to], moveid);
                                if (move.Attacker == null) {
                                    issues.Add(c);
                                    issues.Add(i);
                                    throw new ArgumentException($"Illegal move! at game {c + 1} at move {i - 1}");
                                }
                                b.MakeMove(move);
                                //if (c == 3206) {   //3206 7064 20109 21922
                                //    if (inverted)
                                //        move = move.GetInvertedMove();
                                //    Console.WriteLine(move);
                                //}
                                b.Invert();
                                inverted = !inverted;
                            }
                        }
                        catch {
                            Console.WriteLine($"Issue(s) on line {c}");
                        }
                        sw.Flush();
                        c++;
                        if (c % 100 == 0)
                            Console.WriteLine("Finished processing " + c + " games.");

                        line = sr.ReadLine();
                    }
                }
            }
            foreach (int i in issues)
                Console.WriteLine(i);
            Console.ReadLine();
        }

        static void ExtractFullGames() {
            string current = "";
            try {
                string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/files").Where(x => x.EndsWith(".xml")).ToArray();
                Console.WriteLine("Found " + files.Length + " xml files in folder.");

                StreamWriter sw = new StreamWriter("result.txt");
                int i = 0;
                foreach (string path in files) {
                    current = path;
                    using (var s = System.Xml.XmlReader.Create(path)) {
                        s.MoveToContent();
                        s.ReadToDescendant("game");
                        s.ReadToDescendant("field");
                        string field = s.GetAttribute("content");
                        string p1 = field.Substring(0, 40);
                        string p2 = field.Substring(60, 40);
                        p2 = new string(p2.Reverse().ToArray());
                        p1 = ConvertSetup(p1);
                        p2 = ConvertSetup(p2);

                        sw.Write($"{p1};{p2}");
                        bool inverted = false;
                        string lastid = "";
                        while (s.ReadToNextSibling("move")) {
                            string id = s.GetAttribute("id");
                            if (id == lastid)
                                continue;
                            lastid = id;
                            string src = s.GetAttribute("source");
                            string dst = s.GetAttribute("target");

                            int colid1 = src[0] - 'A';
                            int colid2 = dst[0] - 'A';
                            int rowid1 = src[1] - '1';
                            int rowid2 = dst[1] - '1';
                            // Subtract one to correct for K not being J
                            if (colid1 >= 9)
                                colid1--;
                            if (colid2 >= 9)
                                colid2--;

                            Square origin = Board.GetSquare(rowid1, colid1);
                            Square destination = Board.GetSquare(rowid2, colid2);

                            if (inverted) {
                                origin = 91 - origin;
                                destination = 91 - destination;
                            }

                            int index = Board.GetIndexOfMove(origin, destination);
                            //Debug
                            //Console.WriteLine(Board.GetMoveAtIndex(index));

                            sw.Write($";{index}");
                            inverted = !inverted;
                        }
                        sw.WriteLine();
                        sw.Flush();
                        i++;
                        if (i % 100 == 0)
                            Console.WriteLine("Finished processing " + i + " files.");
                    }
                }

                files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/files").Where(x => x.EndsWith(".gsn")).ToArray();
                Console.WriteLine("Found " + files.Length + " gsn files in folder.");
                foreach (string path in files) {
                    current = path;
                    using (var s = new StreamReader(path)) {
                        s.ReadLine();
                        s.ReadLine();
                        string field = s.ReadLine();
                        string p1 = field.Substring(0, 40);
                        string p2 = field.Substring(60, 40);
                        p2 = new string(p2.Reverse().ToArray());
                        p1 = ConvertSetup(p1);
                        p2 = ConvertSetup(p2);

                        sw.Write($"{p1};{p2}");
                        bool inverted = false;
                        string line = s.ReadLine();
                        while (line != "END") {
                            int colid1 = line[0] - 'A';
                            int colid2 = line[3] - 'A';
                            int rowid1 = line[1] - '1';
                            int rowid2 = line[4] - '1';
                            // Subtract one to correct for K not being J
                            if (colid1 >= 9)
                                colid1--;
                            if (colid2 >= 9)
                                colid2--;

                            Square origin = Board.GetSquare(rowid1, colid1);
                            Square destination = Board.GetSquare(rowid2, colid2);

                            if (inverted) {
                                origin = 91 - origin;
                                destination = 91 - destination;
                            }

                            int index = Board.GetIndexOfMove(origin, destination);
                            //Debug
                            //Console.WriteLine(Board.GetMoveAtIndex(index));

                            sw.Write($";{index}");

                            line = s.ReadLine();
                            inverted = !inverted;
                        }
                        sw.WriteLine();

                        sw.Flush();
                        i++;
                        if (i % 100 == 0)
                            Console.WriteLine("Finished processing " + i + " files.");
                    }
                }


                Console.WriteLine("Finished processing " + i + " files!");
                Console.ReadLine();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(current);
                Console.ReadLine();
            }
        }

        static void ExtractFullGamesWithResultsNoTies() {
            string current = "";
            try {
                string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/files").Where(x => x.EndsWith(".xml")).ToArray();
                Console.WriteLine("Found " + files.Length + " xml files in folder.");

                StreamWriter sw = new StreamWriter("DBWRNT.txt");
                int i = 0;
                StringBuilder sb = new StringBuilder();
                foreach (string path in files) {
                    current = path;
                    sb.Clear();
                    using (var s = System.Xml.XmlReader.Create(path)) {
                        
                        s.MoveToContent();
                        s.ReadToDescendant("game");
                        s.ReadToDescendant("field");
                        string field = s.GetAttribute("content");
                        string p1 = field.Substring(0, 40);
                        string p2 = field.Substring(60, 40);
                        p2 = new string(p2.Reverse().ToArray());
                        p1 = ConvertSetup(p1);
                        p2 = ConvertSetup(p2);

                        
                        
                        
                        bool inverted = false;
                        string lastid = "";
                        try {
                            while (s.Read()) {
                                if (s.IsStartElement()) {
                                    if (s.Name != "move") {
                                        break;
                                    }
                                    string id = s.GetAttribute("id");
                                    if (id == lastid)
                                        continue;
                                    lastid = id;
                                    string src = s.GetAttribute("source");
                                    string dst = s.GetAttribute("target");

                                    int colid1 = src[0] - 'A';
                                    int colid2 = dst[0] - 'A';
                                    int rowid1 = src[1] - '1';
                                    int rowid2 = dst[1] - '1';
                                    // Subtract one to correct for K not being J
                                    if (colid1 >= 9)
                                        colid1--;
                                    if (colid2 >= 9)
                                        colid2--;

                                    Square origin = Board.GetSquare(rowid1, colid1);
                                    Square destination = Board.GetSquare(rowid2, colid2);

                                    if (inverted) {
                                        origin = 91 - origin;
                                        destination = 91 - destination;
                                    }

                                    int index = Board.GetIndexOfMove(origin, destination);
                                    //Debug
                                    //Console.WriteLine(Board.GetMoveAtIndex(index));

                                    sb.Append($";{index}");
                                    inverted = !inverted;
                                }
                            }
                        }
                        catch(NullReferenceException e) {
                            continue;
                        }
                        s.ReadToNextSibling("result");
                        string type = s.GetAttribute("type");
                        type = ResultTypeToResult(type[0]);
                        string winner = s.GetAttribute("winner");
                        if (winner[0] == '3')
                            continue;

                        sw.Write($"{((winner[0] - '0') == 1 ? "0" : "1")};{p1};{p2}");
                        sw.Write(sb.ToString());
                        sw.WriteLine();
                        sw.Flush();
                    }

                    i++;
                    if (i % 100 == 0)
                        Console.WriteLine("Finished processing " + i + " files.");
                }

                files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/files").Where(x => x.EndsWith(".gsn")).ToArray();
                Console.WriteLine("Found " + files.Length + " gsn files in folder.");
                foreach (string path in files) {
                    current = path;
                    using (var s = new StreamReader(path)) {
                        long pos = s.BaseStream.Position;

                        while (s.ReadLine() != "END") { }
                        string resultline = s.ReadLine();
                        string type = resultline.Substring(resultline.Length - 10, 1);
                        type = ResultTypeToResult(type[0]);
                        string winner = resultline.Substring(resultline.Length - 1, 1);
                        string p1result = WinnerToResult(1, (char)(winner[0] + 1));
                        string p2result = WinnerToResult(2, (char)(winner[0] + 1));


                        if ((char)(winner[0] + 1) == '3')
                            continue;
                        string win = (char)(winner[0] + 1) == 1 ? "0" : "1";

                        s.BaseStream.Position = pos;
                        s.DiscardBufferedData();

                        s.ReadLine();
                        s.ReadLine();
                        string field = s.ReadLine();
                        string p1 = field.Substring(0, 40);
                        string p2 = field.Substring(60, 40);
                        p2 = new string(p2.Reverse().ToArray());
                        p1 = ConvertSetup(p1);
                        p2 = ConvertSetup(p2);

                        sw.Write($"{win};{p1};{p2}");
                        bool inverted = false;
                        string line = s.ReadLine();
                        while (line != "END") {
                            int colid1 = line[0] - 'A';
                            int colid2 = line[3] - 'A';
                            int rowid1 = line[1] - '1';
                            int rowid2 = line[4] - '1';
                            // Subtract one to correct for K not being J
                            if (colid1 >= 9)
                                colid1--;
                            if (colid2 >= 9)
                                colid2--;

                            Square origin = Board.GetSquare(rowid1, colid1);
                            Square destination = Board.GetSquare(rowid2, colid2);

                            if (inverted) {
                                origin = 91 - origin;
                                destination = 91 - destination;
                            }

                            int index = Board.GetIndexOfMove(origin, destination);
                            //Debug
                            //Console.WriteLine(Board.GetMoveAtIndex(index));

                            sw.Write($";{index}");

                            line = s.ReadLine();
                            inverted = !inverted;
                        }
                        sw.WriteLine();

                        sw.Flush();
                        
                    }
                    i++;
                    if (i % 100 == 0)
                        Console.WriteLine("Finished processing " + i + " files.");
                }


                Console.WriteLine("Finished processing " + i + " files!");
                Console.ReadLine();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(current);
                Console.ReadLine();
            }
        }

        static void ExtractSetupResults() {
            string current = "";
            try {
                string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/files").Where(x => x.EndsWith(".xml")).ToArray();
                Console.WriteLine("Found " + files.Length + " xml files in folder.");

                StreamWriter sw = new StreamWriter("result.txt");
                int i = 0;
                foreach (string path in files) {
                    current = path;
                    using (var s = System.Xml.XmlReader.Create(path)) {
                        s.MoveToContent();
                        s.ReadToDescendant("game");
                        s.ReadToDescendant("field");
                        string field = s.GetAttribute("content");
                        string p1 = field.Substring(0, 40);
                        string p2 = field.Substring(60, 40);
                        p2 = new string(p2.Reverse().ToArray());
                        p1 = ConvertSetup(p1);
                        p2 = ConvertSetup(p2);

                        s.ReadToNextSibling("result");
                        string type = s.GetAttribute("type");
                        type = ResultTypeToResult(type[0]);
                        string winner = s.GetAttribute("winner");
                        string p1result = WinnerToResult(1, winner[0]);
                        string p2result = WinnerToResult(2, winner[0]);
                        //Console.WriteLine(p1);
                        //Console.WriteLine(p2);

                        sw.WriteLine(p1 + ";" + type + ";" + p1result);
                        sw.WriteLine(p2 + ";" + type + ";" + p2result);
                        sw.Flush();
                        i++;
                        if (i % 100 == 0)
                            Console.WriteLine("Finished processing " + i + " files.");
                    }
                }

                files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/files").Where(x => x.EndsWith(".gsn")).ToArray();
                Console.WriteLine("Found " + files.Length + " gsn files in folder.");
                foreach (string path in files) {
                    current = path;
                    using (var s = new StreamReader(path)) {
                        s.ReadLine();
                        s.ReadLine();
                        string field = s.ReadLine();
                        string p1 = field.Substring(0, 40);
                        string p2 = field.Substring(60, 40);
                        p2 = new string(p2.Reverse().ToArray());
                        p1 = ConvertSetup(p1);
                        p2 = ConvertSetup(p2);

                        while (s.ReadLine() != "END") { }
                        string resultline = s.ReadLine();
                        string type = resultline.Substring(resultline.Length - 10, 1);
                        type = ResultTypeToResult(type[0]);
                        string winner = resultline.Substring(resultline.Length - 1, 1);
                        string p1result = WinnerToResult(1, (char)(winner[0] + 1));
                        string p2result = WinnerToResult(2, (char)(winner[0] + 1));
                        //Console.WriteLine(p1);
                        //Console.WriteLine(p2);
                        sw.WriteLine(p1 + ";" + type + ";" + p1result);
                        sw.WriteLine(p2 + ";" + type + ";" + p2result);
                        sw.Flush();
                        i++;
                        if (i % 100 == 0)
                            Console.WriteLine("Finished processing " + i + " files.");
                    }
                }


                Console.WriteLine("Finished processing " + i + " files!");
                Console.ReadLine();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(current);
                Console.ReadLine();
            }
        }

        static void ExtractAllSetups() {
            string current = "";
            try {
                string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/files").Where(x => x.EndsWith(".xml")).ToArray();
                Console.WriteLine("Found " + files.Length + " xml files in folder.");

                StreamWriter sw = new StreamWriter("result.txt");
                int i = 0;
                foreach (string path in files) {
                    current = path;
                    using (var s = System.Xml.XmlReader.Create(path)) {
                        s.MoveToContent();
                        s.ReadToDescendant("game");
                        s.ReadToDescendant("field");
                        string field = s.GetAttribute("content");
                        string p1 = field.Substring(0, 40);
                        string p2 = field.Substring(60, 40);
                        p2 = new string(p2.Reverse().ToArray());
                        p1 = ConvertSetup(p1);
                        p2 = ConvertSetup(p2);
                        //Console.WriteLine(p1);
                        //Console.WriteLine(p2);
                        sw.Write(p1);
                        sw.Write(p2);
                        sw.Flush();
                        i++;
                        if (i % 100 == 0)
                            Console.WriteLine("Finished processing " + i + " files.");
                    }
                }

                files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/files").Where(x => x.EndsWith(".gsn")).ToArray();
                Console.WriteLine("Found " + files.Length + " gsn files in folder.");
                foreach (string path in files) {
                    current = path;
                    using (var s = new StreamReader(path)) {
                        s.ReadLine();
                        s.ReadLine();
                        string field = s.ReadLine();
                        string p1 = field.Substring(0, 40);
                        string p2 = field.Substring(60, 40);
                        p2 = new string(p2.Reverse().ToArray());
                        p1 = ConvertSetup(p1);
                        p2 = ConvertSetup(p2);
                        //Console.WriteLine(p1);
                        //Console.WriteLine(p2);
                        sw.Write(p1);
                        sw.Write(p2);
                        sw.Flush();
                        i++;
                        if (i % 100 == 0)
                            Console.WriteLine("Finished processing " + i + " files.");
                    }
                }

                Console.WriteLine("Finished processing " + i + " files!");
                Console.ReadLine();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(current);
                Console.ReadLine();
            }
        }


        static string ConvertSetup(string s) {
            string newstring = "";
            for (int i = 0; i < s.Length; i++)
                newstring += SymbolToUnitSymbol(s[i]);
            return newstring;
        }

        static string Deconvert(string s, bool asBlue) {
            string newstring = "";
            for (int i = 0; i < s.Length; i++)
                newstring += ReverseUnitSymbol(s[asBlue ? 39 - i : i], asBlue);
            return newstring;
        }

        static string WinnerToResult(int player, char winner) {
            if (player == (winner - '0'))
                return "Victory";
            if (winner == '3')
                return "Tie";
            return "Defeat";
        }

        static string ResultTypeToResult(char c) {
            return c switch {
                '0' => "Win - No Moves Left",
                '1' => "Win - Flag Capture",
                '2' => "Forced Tie",
                '3' => "Forfeit",
                '4' => "Agreed tie",
                _ => throw new ArgumentException("Unknown result type! " + c)
            };
        }

        static Rank CharToRank(char c) {
            return c switch {
                'B' => Rank.Bomb,
                'F' => Rank.Flag,
                '1' => Rank.Spy,
                '2' => Rank.Scout,
                '3' => Rank.Miner,
                '4' => Rank.Sergeant,
                '5' => Rank.Lieutenant,
                '6' => Rank.Captain,
                '7' => Rank.Major,
                '8' => Rank.Colonel,
                '9' => Rank.General,
                'M' => Rank.Marshal,
                _ => throw new ArgumentException("Unknown character! " + c)
            };
        }

        static char RankToChar(Rank r) {
            return r switch {
                Rank.Bomb => 'B',
                Rank.Flag => 'F',
                Rank.Spy => '1',
                Rank.Scout => '2',
                Rank.Miner => '3',
                Rank.Sergeant => '4',
                Rank.Lieutenant => '5',
                Rank.Captain => '6',
                Rank.Major => '7',
                Rank.Colonel => '8',
                Rank.General => '9',
                Rank.Marshal => 'M',
                _ => throw new ArgumentException("Unknown rank! " + r)
            };
        }

        static char SymbolToUnitSymbol(char c) {
            return c switch {
                'A' => throw new ArgumentException("Empty field in setup found!"),
                'B' => 'B',
                'C' => '1',
                'D' => '2',
                'E' => '3',
                'F' => '4',
                'G' => '5',
                'H' => '6',
                'I' => '7',
                'J' => '8',
                'K' => '9',
                'L' => 'M',
                'M' => 'F',
                'N' => 'B',
                'O' => '1',
                'P' => '2',
                'Q' => '3',
                'R' => '4',
                'S' => '5',
                'T' => '6',
                'U' => '7',
                'V' => '8',
                'W' => '9',
                'X' => 'M',
                'Y' => 'F',
                _ => throw new ArgumentException("Unknown symbol encountered! " + c)
            };
        }

        static char ReverseUnitSymbol(char c, bool asBlue = false) {
            if (!asBlue)
                return c switch {
                    'B' => 'B',
                    '1' => 'C',
                    '2' => 'D',
                    '3' => 'E',
                    '4' => 'F',
                    '5' => 'G',
                    '6' => 'H',
                    '7' => 'I',
                    '8' => 'J',
                    '9' => 'K',
                    'M' => 'L',
                    'F' => 'M',
                    _ => throw new ArgumentException("Unknown symbol encountered! " + c)
                };
            else
                return c switch {
                    'B' => 'N',
                    '1' => 'O',
                    '2' => 'P',
                    '3' => 'Q',
                    '4' => 'R',
                    '5' => 'S',
                    '6' => 'T',
                    '7' => 'U',
                    '8' => 'V',
                    '9' => 'W',
                    'M' => 'X',
                    'F' => 'Y',
                    _ => throw new ArgumentException("Unknown symbol encountered! " + c)
                };
        }
    }
}
