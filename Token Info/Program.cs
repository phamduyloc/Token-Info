using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;

namespace Info_Token
{
    class Program
    {
        private struct sLibraryInfo
        {
            public string Manufacturer;
            public string Description;
            public string Version;
        }

        private struct sSlotInfo
        {
            public string Manufacturer;
            public string Description;
            public string Token_Present;
        }

        private struct sTokenPresentInfo
        {
            public string Manufacturer;
            public string Model;
            public string Serialnumber;
            public string Label;
            public string FreePrivateMemory;
            public string FreePublicMemory;
        }

        private static sLibraryInfo sli;
        private static sSlotInfo[] sis;
        private static sTokenPresentInfo[] tpis;
        static void Main(string[] args)
        {
            string pathLibrary = @"C:\windows\system32\st3ace.dll";     //library for token
            GetTokenInfo(pathLibrary);
            Console.WriteLine("[+] Library Info: \n\t" +
                "[-] Manufacturer: " + sli.Manufacturer + "\n\t" +
                "[-] Descripttion: " + sli.Description + "\n\t" +
                "[-] Version: " + sli.Version + "\n\n");
            if (sis.Length > 0)
            {
                for (int i = 0; i < sis.Length; i++)
                {
                    Console.Write("[+] Slot " + i + ": \n\t" +
                        "[-] Slot ManufacturerId: " + sis[i].Manufacturer + "\n\t" +
                        "[-] Slot Description: " + sis[i].Description + "\n\t" +
                        "[-] Token Present: " + sis[i].Token_Present + "\n\n");
                    Console.Write("[+] Token Info: \n\t" +
                        "[-] Token Manufacturer: " + tpis[i].Manufacturer +"\n\t" +
                        "[-] Token Model: " + tpis[i].Model + "\n\t" +
                        "[-] Token SerialNumber: " + tpis[i].Serialnumber + "\n\t" +
                        "[-] Token Label: " + tpis[i].Label + "\n\n");
                }
            }
            Console.Read();
        }
        private static void GetTokenInfo(string pathLibrary)
        {
            sli = new sLibraryInfo();

            using (Pkcs11 pkcs11 = new Pkcs11(pathLibrary, AppType.SingleThreaded))
            {
                LibraryInfo libraryInfo = pkcs11.GetInfo();
                sli.Manufacturer = libraryInfo.ManufacturerId;
                sli.Description = libraryInfo.LibraryDescription;
                sli.Version = libraryInfo.LibraryVersion;

                //Get list of all available slots
                List<Slot> slots = pkcs11.GetSlotList(SlotsType.WithTokenPresent);
                SlotInfo slotInfo = null;
                TokenInfo tokenInfo = null;
                sis = new sSlotInfo[slots.Count];
                tpis = new sTokenPresentInfo[slots.Count];
                for (int i = 0; i < slots.Count; i++)
                {
                    slotInfo = slots[i].GetSlotInfo();
                    sis[i].Manufacturer = slotInfo.ManufacturerId;
                    sis[i].Description = slotInfo.SlotDescription;
                    sis[i].Token_Present = slotInfo.SlotFlags.TokenPresent.ToString();
                    if (slotInfo.SlotFlags.TokenPresent)
                    {
                        tokenInfo = slots[i].GetTokenInfo();
                        tpis[i].Manufacturer = tokenInfo.ManufacturerId.ToString().Substring(0, tokenInfo.ManufacturerId.ToString().Length - 1);
                        tpis[i].Model = tokenInfo.Model;
                        tpis[i].Serialnumber = tokenInfo.SerialNumber;
                        tpis[i].Label = tokenInfo.Label;
                        tpis[i].FreePrivateMemory = tokenInfo.FreePrivateMemory.ToString();
                        tpis[i].FreePublicMemory = tokenInfo.FreePublicMemory.ToString();
                    }
                }
            }
        }
    }
}
