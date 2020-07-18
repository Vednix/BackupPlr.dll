using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Terraria;
using Terraria.IO;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace BackupPlr
{
    [ApiVersion(2, 1)]
    public class SaveMain : TerrariaPlugin

    #region Info

    {
        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public override string Name
        {
            get { return "BackupPlr for Mobile 1.3.0"; }
        }

        public override string Author
        {
            get { return "Vednix"; }
        }

        public override string Description
        {
            get { return "Auto backup .plr connecting to server"; }
        }

        public SaveMain(Main game)
            : base(game)
        {
        }
        private enum SaveType
        {
            AccountCreate = 0,
            PreLogin = 1
        }
        #endregion
        private static string SaveLocation = $"{Path.Combine(TShock.SavePath, "BackupPlr")}";
        public static string DateTimeNow => DateTime.Now.ToString("dd-MM-yyyy--HH-mm-ss");
        public override void Initialize()
        {
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
        }
        private void OnJoin(JoinEventArgs args)
        {
            if (args.Who < 0)
                return;
            Player player = TShock.Players[args.Who].TPlayer;
            try
            {

                if (!Directory.Exists(SaveLocation))
                    Directory.CreateDirectory(SaveLocation);

                Terraria.IO.PlayerFileData playerFile = new Terraria.IO.PlayerFileData();
                playerFile.Metadata = FileMetadata.FromCurrentSettings(FileType.Player);

                string SaveLocationForPlr = Path.Combine(SaveLocation, $"{player.name}");
                if (!Directory.Exists(SaveLocationForPlr))
                    Directory.CreateDirectory(SaveLocationForPlr);

                var file = Path.Combine(SaveLocationForPlr, $"{player.name}--{DateTimeNow}");

                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    using (var binaryWriter = new BinaryWriter(fileStream))
                    {
                        binaryWriter.Write(Main.curRelease);
                        playerFile.Metadata.Write(binaryWriter);
                        binaryWriter.Write(player.name);
                        binaryWriter.Write(player.difficulty);
                        binaryWriter.Write(playerFile.GetPlayTime().Ticks);
                        binaryWriter.Write(player.hair);
                        binaryWriter.Write(player.hairDye);
                        BitsByte bb = 0;
                        for (int i = 0; i < 8; i++)
                        {
                            bb[i] = player.hideVisual[i];
                        }
                        binaryWriter.Write(bb);
                        bb = 0;
                        for (int j = 0; j < 2; j++)
                        {
                            bb[j] = player.hideVisual[j + 8];
                        }
                        binaryWriter.Write(bb);
                        binaryWriter.Write(player.hideMisc);
                        binaryWriter.Write((byte)player.skinVariant);
                        binaryWriter.Write(player.statLife);
                        binaryWriter.Write(player.statLifeMax);
                        binaryWriter.Write(player.statMana);
                        binaryWriter.Write(player.statManaMax);
                        binaryWriter.Write(player.extraAccessory);
                        binaryWriter.Write(player.taxMoney);
                        binaryWriter.Write(player.hairColor.R);
                        binaryWriter.Write(player.hairColor.G);
                        binaryWriter.Write(player.hairColor.B);
                        binaryWriter.Write(player.skinColor.R);
                        binaryWriter.Write(player.skinColor.G);
                        binaryWriter.Write(player.skinColor.B);
                        binaryWriter.Write(player.eyeColor.R);
                        binaryWriter.Write(player.eyeColor.G);
                        binaryWriter.Write(player.eyeColor.B);
                        binaryWriter.Write(player.shirtColor.R);
                        binaryWriter.Write(player.shirtColor.G);
                        binaryWriter.Write(player.shirtColor.B);
                        binaryWriter.Write(player.underShirtColor.R);
                        binaryWriter.Write(player.underShirtColor.G);
                        binaryWriter.Write(player.underShirtColor.B);
                        binaryWriter.Write(player.pantsColor.R);
                        binaryWriter.Write(player.pantsColor.G);
                        binaryWriter.Write(player.pantsColor.B);
                        binaryWriter.Write(player.shoeColor.R);
                        binaryWriter.Write(player.shoeColor.G);
                        binaryWriter.Write(player.shoeColor.B);
                        for (int k = 0; k < player.armor.Length; k++)
                        {
                            if (player.armor[k].Name == null)
                            {
                                player.armor[k].SetNameOverride("");
                            }
                            binaryWriter.Write(player.armor[k].netID);
                            binaryWriter.Write(player.armor[k].prefix);
                        }
                        for (int l = 0; l < player.dye.Length; l++)
                        {
                            binaryWriter.Write(player.dye[l].netID);
                            binaryWriter.Write(player.dye[l].prefix);
                        }
                        for (int m = 0; m < 58; m++)
                        {
                            if (player.inventory[m].Name == null)
                            {
                                player.inventory[m].SetNameOverride("");
                            }
                            binaryWriter.Write(player.inventory[m].netID);
                            binaryWriter.Write(player.inventory[m].stack);
                            binaryWriter.Write(player.inventory[m].prefix);
                            binaryWriter.Write(player.inventory[m].favorited);
                        }
                        for (int n = 0; n < player.miscEquips.Length; n++)
                        {
                            binaryWriter.Write(player.miscEquips[n].netID);
                            binaryWriter.Write(player.miscEquips[n].prefix);
                            binaryWriter.Write(player.miscDyes[n].netID);
                            binaryWriter.Write(player.miscDyes[n].prefix);
                        }
                        for (int num = 0; num < 40; num++)
                        {
                            if (player.bank.item[num].Name == null)
                            {
                                player.bank.item[num].SetNameOverride("");
                            }
                            binaryWriter.Write(player.bank.item[num].netID);
                            binaryWriter.Write(player.bank.item[num].stack);
                            binaryWriter.Write(player.bank.item[num].prefix);
                        }
                        for (int num2 = 0; num2 < 40; num2++)
                        {
                            if (player.bank2.item[num2].Name == null)
                            {
                                player.bank2.item[num2].SetNameOverride("");
                            }
                            binaryWriter.Write(player.bank2.item[num2].netID);
                            binaryWriter.Write(player.bank2.item[num2].stack);
                            binaryWriter.Write(player.bank2.item[num2].prefix);
                        }
                        for (int num3 = 0; num3 < 22; num3++)
                        {
                            if (Main.buffNoSave[player.buffType[num3]])
                            {
                                binaryWriter.Write(0);
                                binaryWriter.Write(0);
                            }
                            else
                            {
                                binaryWriter.Write(player.buffType[num3]);
                                binaryWriter.Write(player.buffTime[num3]);
                            }
                        }
                        for (int num4 = 0; num4 < 200; num4++)
                        {
                            if (player.spN[num4] == null)
                            {
                                binaryWriter.Write(-1);
                                break;
                            }
                            binaryWriter.Write(player.spX[num4]);
                            binaryWriter.Write(player.spY[num4]);
                            binaryWriter.Write(player.spI[num4]);
                            binaryWriter.Write(player.spN[num4]);
                        }
                        binaryWriter.Write(player.hbLocked);
                        for (int num5 = 0; num5 < player.hideInfo.Length; num5++)
                        {
                            binaryWriter.Write(player.hideInfo[num5]);
                        }
                        binaryWriter.Write(player.anglerQuestsFinished);
                        binaryWriter.Flush();
                        //cryptoStream.FlushFinalBlock();
                    }
                }
                EncryptFile(file, file + ".plr");
                File.Delete(file);
                TShock.Log.ConsoleInfo($"[BackupPlr] {player.name} => Backup Done!");
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleInfo($"[BackupPlr] {player.name} => Backup Fail!");
                TShock.Log.ConsoleError(ex.ToString());
            }
        }

        private static void EncryptFile(string inputFile, string outputFile)
        {
            const string s = "h3y_gUyZ";
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            byte[] bytes = unicodeEncoding.GetBytes(s);
            FileStream fileStream = new FileStream(outputFile, FileMode.Create);
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            CryptoStream cryptoStream = new CryptoStream(fileStream, rijndaelManaged.CreateEncryptor(bytes, bytes),
                CryptoStreamMode.Write);
            FileStream fileStream2 = new FileStream(inputFile, FileMode.Open);
            int num;
            while ((num = fileStream2.ReadByte()) != -1)
            {
                cryptoStream.WriteByte((byte)num);
            }
            fileStream2.Close();
            cryptoStream.Close();
            fileStream.Close();
        }
    }
}