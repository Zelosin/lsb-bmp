using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Lr4
{
    class Program
    {
        private static byte BitToByte(BitArray scr)
        {
            byte num = 0;
            for (int i = 0; i < scr.Count; i++)
                if (scr[i] == true)
                    num += (byte)Math.Pow(2, i);
            return num;
        }
        private static BitArray ByteToBit(byte src)
        {
            BitArray bitArray = new BitArray(8);
            bool st = false;
            for (int i = 0; i < 8; i++)
            {
                if ((src >> i & 1) == 1)
                {
                    st = true;
                }
                else st = false;
                bitArray[i] = st;
            }
            return bitArray;
        }
        private static bool isEncryption(Bitmap scr)
        {
            byte[] rez = new byte[1];
            Color color = scr.GetPixel(0, 0);
            BitArray colorArray = ByteToBit(color.R); 
            BitArray messageArray = ByteToBit(color.R);
            messageArray[0] = colorArray[0];
            messageArray[1] = colorArray[1];

            colorArray = ByteToBit(color.G);
            messageArray[2] = colorArray[0];
            messageArray[3] = colorArray[1];
            messageArray[4] = colorArray[2];

            colorArray = ByteToBit(color.B);
            messageArray[5] = colorArray[0];
            messageArray[6] = colorArray[1];
            messageArray[7] = colorArray[2];
            rez[0] = BitToByte(messageArray); 
            string m = Encoding.GetEncoding(1251).GetString(rez);
            if (m == "/")
            {
                return true;
            }
            else return false;
        }
        private static void WriteCountText(int count, Bitmap src)
        {
            byte[] CountSymbols = Encoding.GetEncoding(1251).GetBytes(count.ToString());
            for (int i = 0; i < 3; i++)
            {
                BitArray bitCount = ByteToBit(CountSymbols[i]); 
                Color pColor = src.GetPixel(0, i + 1); 
                BitArray bitsCurColor = ByteToBit(pColor.R); 
                bitsCurColor[0] = bitCount[0];
                bitsCurColor[1] = bitCount[1];
                byte nR = BitToByte(bitsCurColor); 

                bitsCurColor = ByteToBit(pColor.G);
                bitsCurColor[0] = bitCount[2];
                bitsCurColor[1] = bitCount[3];
                bitsCurColor[2] = bitCount[4];
                byte nG = BitToByte(bitsCurColor);

                bitsCurColor = ByteToBit(pColor.B);
                bitsCurColor[0] = bitCount[5];
                bitsCurColor[1] = bitCount[6];
                bitsCurColor[2] = bitCount[7];
                byte nB = BitToByte(bitsCurColor);

                Color nColor = Color.FromArgb(nR, nG, nB); 
                src.SetPixel(0, i + 1, nColor); 
            }
        }
        private static int ReadCountText(Bitmap src)
        {
            byte[] rez = new byte[3]; 
            for (int i = 0; i < 3; i++)
            {
                Color color = src.GetPixel(0, i + 1); 
                BitArray colorArray = ByteToBit(color.R); 
                BitArray bitCount = ByteToBit(color.R); ; 
                bitCount[0] = colorArray[0];
                bitCount[1] = colorArray[1];

                colorArray = ByteToBit(color.G);
                bitCount[2] = colorArray[0];
                bitCount[3] = colorArray[1];
                bitCount[4] = colorArray[2];

                colorArray = ByteToBit(color.B);
                bitCount[5] = colorArray[0];
                bitCount[6] = colorArray[1];
                bitCount[7] = colorArray[2];
                rez[i] = BitToByte(bitCount);
            }
            string m = Encoding.GetEncoding(1251).GetString(rez);
            return Convert.ToInt32(m, 10);
        }
        private static bool WriteText(FileStream rText, FileStream picSrc)
        {
            Bitmap src = new Bitmap(picSrc);

            BinaryReader bText = new BinaryReader(rText, Encoding.ASCII);

            List<byte> bList = new List<byte>();
            while (bText.PeekChar() != -1)
            {
                bList.Add(bText.ReadByte());
            }
            int CountText = bList.Count;
            bText.Close();

            byte[] Symbol = Encoding.GetEncoding(1251).GetBytes("/");
            BitArray ArrBeginSymbol = ByteToBit(Symbol[0]);
            Color curColor = src.GetPixel(0, 0);
            BitArray tempArray = ByteToBit(curColor.R);
            tempArray[0] = ArrBeginSymbol[0];
            tempArray[1] = ArrBeginSymbol[1];
            byte nR = BitToByte(tempArray);

            tempArray = ByteToBit(curColor.G);
            tempArray[0] = ArrBeginSymbol[2];
            tempArray[1] = ArrBeginSymbol[3];
            tempArray[2] = ArrBeginSymbol[4];
            byte nG = BitToByte(tempArray);

            tempArray = ByteToBit(curColor.B);
            tempArray[0] = ArrBeginSymbol[5];
            tempArray[1] = ArrBeginSymbol[6];
            tempArray[2] = ArrBeginSymbol[7];
            byte nB = BitToByte(tempArray);

            Color nColor = Color.FromArgb(nR, nG, nB);
            src.SetPixel(0, 0, nColor);

            WriteCountText(CountText, src);

            int index = 0;
            bool st = false;
            for (int i = 4; i < src.Width; i++)
            {
                for (int j = 0; j < src.Height; j++)
                {
                    Color pixelColor = src.GetPixel(i, j);
                    if (index == bList.Count)
                    {
                        st = true;
                        break;
                    }
                    BitArray colorArray = ByteToBit(pixelColor.R);
                    BitArray messageArray = ByteToBit(bList[index]);
                    colorArray[0] = messageArray[0];
                    colorArray[1] = messageArray[1];
                    byte newR = BitToByte(colorArray);

                    colorArray = ByteToBit(pixelColor.G);
                    colorArray[0] = messageArray[2];
                    colorArray[1] = messageArray[3];
                    colorArray[2] = messageArray[4];
                    byte newG = BitToByte(colorArray);

                    colorArray = ByteToBit(pixelColor.B);
                    colorArray[0] = messageArray[5];
                    colorArray[1] = messageArray[6];
                    colorArray[2] = messageArray[7];
                    byte newB = BitToByte(colorArray);

                    Color newColor = Color.FromArgb(newR, newG, newB);
                    src.SetPixel(i, j, newColor);
                    index++;
                }
                if (st)
                {
                    break;
                }
            }
            src.Save(picSrc, System.Drawing.Imaging.ImageFormat.Bmp);
            return true;
        }
        private static bool ReadText(FileStream rText, Bitmap src) {
            int countSymbol = ReadCountText(src); 
            byte[] message = new byte[countSymbol];
            int index = 0;
            bool st = false;
            for (int i = 4; i < src.Width; i++)
            {
                for (int j = 0; j < src.Height; j++)
                {
                    Color pixelColor = src.GetPixel(i, j);
                    if (index == message.Length)
                    {
                        st = true;
                        break;
                    }
                    BitArray colorArray = ByteToBit(pixelColor.R);
                    BitArray messageArray = ByteToBit(pixelColor.R); ;
                    messageArray[0] = colorArray[0];
                    messageArray[1] = colorArray[1];

                    colorArray = ByteToBit(pixelColor.G);
                    messageArray[2] = colorArray[0];
                    messageArray[3] = colorArray[1];
                    messageArray[4] = colorArray[2];

                    colorArray = ByteToBit(pixelColor.B);
                    messageArray[5] = colorArray[0];
                    messageArray[6] = colorArray[1];
                    messageArray[7] = colorArray[2];
                    message[index] = BitToByte(messageArray);
                    index++;
                }
                if (st)
                {
                    break;
                }
            }
            string strMessage = Encoding.GetEncoding(1251).GetString(message);
            StreamWriter wText = new StreamWriter(rText, Encoding.Default);
            wText.Write(strMessage);
            wText.Close();
            return true;
        }
        static void Main(string[] args)
        {
            while (true) {
                string[] tokens = Console.ReadLine().Split();
                FileStream textStream;
                FileStream picStream;
                switch (tokens[0]) {
                    case ("-check"): {
                        picStream = new FileStream((tokens[1]), FileMode.Open);
                        if (File.Exists(tokens[1]) && 
                            (isEncryption(new Bitmap(picStream))))
                                Console.WriteLine("В этом bmp-файле содержится информация");
                            else
                                Console.WriteLine("В этом bmp-файле не содержится информация");
                            picStream.Close();
                            break;
                        }
                    case ("-embed"): {
                        picStream = new FileStream((tokens[2]), FileMode.Open);
                        textStream = new FileStream((tokens[1]), FileMode.Open);
                        if (File.Exists(tokens[1]) && File.Exists(tokens[2]) &&
                            WriteText(textStream, picStream))
                            Console.WriteLine("Информация успешно записана в bmp-файл");
                        else
                            Console.WriteLine("Не удалось записать информацию");
                        picStream.Close();
                        textStream.Close();
                        break;
                    }
                    case ("-retrieve"):{
                        picStream = new FileStream((tokens[2]), FileMode.Open);
                        textStream = new FileStream((tokens[1]), FileMode.Open);
                        if (File.Exists(tokens[1]) && File.Exists(tokens[2]) &&
                            ReadText(textStream, new Bitmap(picStream)))
                            Console.WriteLine("Информация успешно считана из bmp-файл");
                        else
                            Console.WriteLine("Не удалось считать информацию");
                        picStream.Close();
                        textStream.Close();
                        break;
                    }                        
                }
            }  
        }
    }
}
