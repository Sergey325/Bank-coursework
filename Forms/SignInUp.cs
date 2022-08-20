using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Курсовая.CustomControls;
using Курсовая.DataModel;
using Курсовая.Properties;

namespace Курсовая
{
    public partial class SignInUp : Form
    {
        private List<Account> accounts;
        private List<List<Deposit>> deposits;
        private List<List<Transaction>> transactions;
        private readonly string accPath = "Accounts.bin";
        private readonly string depPath = "Deposits.bin";
        private readonly string tranPath = "Transactions.bin";

        public SignInUp()
        {
            accounts = DataSerialiser.BinaryDeserialise(accPath) as List<Account>;
            deposits = DataSerialiser.BinaryDeserialise(depPath) as List<List<Deposit>>;
            transactions = DataSerialiser.BinaryDeserialise(tranPath) as List<List<Transaction>>;
            InitializeComponent();
        }

        private void SignInUp_Load(object sender, EventArgs e)
        {
            lbuttonResize.Click += (s, a) =>
            {
                if (WindowState == FormWindowState.Normal)
                {
                    this.WindowState = FormWindowState.Maximized;
                    lbuttonResize.Text = "❐";
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                    lbuttonResize.Text = "❒";
                }
            };
            lbuttonExit.Click += (s, a) => { Application.Exit(); };
            lbuttonRollUp.Click += (s, a) => { WindowState = FormWindowState.Minimized; };
            readSavedLogPass();
            foreach (var cPB in pnlSignUp.Controls.OfType<CustomPictureBox>())
                cPB.Click += (s, a) => ShowBorder(s as CustomPictureBox);
            toggleSwitch1.Click += (s, a) =>
            {
                labelRemembMe.ForeColor = toggleSwitch1.Value ? Color.FromArgb(137, 26, 241) : Color.FromArgb(191, 191, 191);
                if (toggleSwitch1.Value && inputPhoneNumber.text!= "" && inputPassword.Text.Trim() != "")
                    File.WriteAllText("SavedPasswords.txt", $"{inputPhoneNumber.text} {inputPassword.Text}");
                else if (File.Exists("SavedPasswords.txt")) File.Delete("SavedPasswords.txt");
            };
            inputPassword.TextChanged += (s, a) =>
            {
                inputPassword.PasswordChar = '*';
                if (inputPassword.Text.Length == 0) inputPassword.PasswordChar = (char)0;
            };
            inputPhoneNumber.TextChange += (s, a) =>
            {
                CheckInput();
            };
            buttonLogIn.Click += (s, a) =>
            {
                loaderSignIn.Visible = true;
                if (toggleSwitch1.Value)
                    File.WriteAllText("SavedPasswords.txt", $"{inputPhoneNumber.text},{inputPassword.Text}");
                else if (File.Exists("SavedPasswords.txt")) File.Delete("SavedPasswords.txt");
                int accID = accounts.IndexOf(accounts.FirstOrDefault(x => x.phoneNumber == inputPhoneNumber.text && x.password == inputPassword.Text.Trim()));
                if (accID != -1)
                {
                    unlock();
                    MyContext.ShowForm2(accID, Location);
                    Focus();
                }
                else showMessage(1, "Account not found", true);
                loaderSignIn.Visible = false;
            };
            tbFName.TextChanged += (s, a) => CheckInput();
            tbSName.TextChanged += (s, a) => CheckInput();
            tbPassword.TextChanged += (s, a) =>
            {
                tbPassword.PasswordChar = '*';
                if (tbPassword.Text.Length == 0) tbPassword.PasswordChar = (char)0;
                tbConfirm.IconLeft = (Image)Resources.ResourceManager.GetObject(tbConfirm.Text == tbPassword.Text ? "Check_Circle_64px" : "Check_Circle_50px");
                CheckInput();
            };
            tbConfirm.TextChanged += (s, a) =>
            {
                tbConfirm.PasswordChar = '*';
                if (tbConfirm.Text.Length == 0) tbConfirm.PasswordChar = (char)0;
                tbConfirm.IconLeft = (Image)Resources.ResourceManager.GetObject(tbConfirm.Text == tbPassword.Text ? "ok_64px" : "Check_Circle_50px");
                CheckInput();
            };
            btnCreate.Click += (s, a) =>
            {
                accounts.Add(new Account(tbFName.Text, tbSName.Text, tbPassword.Text, inputPhoneNumber.text, accounts.Count, pnlSignUp.Controls.OfType<CustomPictureBox>().First(x => x.BorderSize == 1).Name));
                deposits.Add(new List<Deposit>());
                transactions.Add(new List<Transaction>());
                DataSerialiser.BinarySerialise(accounts, accPath);
                DataSerialiser.BinarySerialise(deposits, depPath);
                DataSerialiser.BinarySerialise(transactions, tranPath);
                showMessage(2, "User has been created", false);
            };
            inputPhoneNumber.Click += (s, a) =>
            {
                if(inputPhoneNumber.text == "")inputPhoneNumber.Mask = "(999) 999-99-99";
            };

            lRegister.MouseEnter += (s, a) =>
            {
                lRegister.ForeColor = Color.FromArgb(137, 26, 241);
            };
            lRegister.Click += (s, a) =>
            {
                WindowState = FormWindowState.Normal;
                lRegister.ForeColor = Color.FromArgb(157, 16, 221);
                pnlSignUp.Controls.AddRange(new Control[] { pnlSignIn.Controls[8], pnlSignIn.Controls[9], pnlSignIn.Controls[10], pnlSignIn.Controls[11] });
                inputPhoneNumber.Location = new Point(532, 291);
                inputPhoneNumber.Mask = "";
                inputPhoneNumber.text = "";
                pages.SelectedIndex = 1;
            };
            lRegister.MouseLeave += (s, a) =>
            {
                lRegister.ForeColor = Color.FromArgb(157, 16, 221);
            };
            lIconBack.MouseEnter += (s, a) => RecolorBack(true);
            lIconBack.Click += (s, a) => BackToSignIn();
            lIconBack.MouseLeave += (s, a) => RecolorBack(false);
            lBack.MouseLeave += (s, a) => RecolorBack(false);
            lBack.MouseEnter += (s, a) => RecolorBack(true);
            lBack.Click += (s, a) => BackToSignIn();
        }
        private void readSavedLogPass()
        {
            if (File.Exists("SavedPasswords.txt"))
            {
                toggleSwitch1.Value = true;
                labelRemembMe.ForeColor = Color.FromArgb(137, 26, 241);
                var str = File.ReadAllText("SavedPasswords.txt").Split(',');
                inputPhoneNumber.Mask = "(999) 999-99-99";
                inputPhoneNumber.text = str[0];
                inputPassword.Text = str[1];
                inputPassword.PasswordChar = '*';
                ActiveControl = null;
            }
        }
        private async void showMessage(int sec, string msg, bool SignIn)
        {
            if (SignIn)
            {
                lSignInInfo.Text = msg;
                await Task.Delay(sec * 1000);
                lSignInInfo.Text = "";
            }
            else
            {
                lSignUpInfo.Text = msg;
                await Task.Delay(sec * 1000);
                lSignUpInfo.Text = "";
            }
        }
        private async void unlock()
        {
            await Task.Run(() =>
            {
                animation1.Hide(pnlSignIn);
            });
            for (Opacity = 1; Opacity > 0; Opacity -= 0.015) await Task.Delay(1);
            
            Close();
        }
        private void ShowBorder(CustomPictureBox pb)
        {
            ActiveControl = null;
            if (pnlSignUp.Controls.OfType<CustomPictureBox>().Any(x => x.BorderSize == 1))
            {
                pnlSignUp.Controls.OfType<CustomPictureBox>().First(x => x.BorderSize == 1).BorderSize = 0;
            }
            pb.BorderSize = 1;
            CheckInput();
        }
        private void CheckInput()
        {
            btnCreate.Visible = tbSName.Text.Length > 0 && tbFName.Text.Length > 0 && tbPassword.Text.Length > 0 && tbConfirm.Text == tbPassword.Text
                && pnlSignUp.Controls.OfType<CustomPictureBox>().Any(x => x.BorderSize == 1) && inputPhoneNumber.Mask != "" 
                && inputPhoneNumber.text.Length == 15 && inputPhoneNumber.text.Count(x => x == ' ') == 1 && !accounts.Any(x => x.phoneNumber == inputPhoneNumber.text);
        }
        private void RecolorBack(bool purple)
        {
            if (purple)
            {
                lIconBack.BackgroundImage = (Image)Resources.ResourceManager.GetObject("back_50px_purple");
                lBack.ForeColor = Color.FromArgb(137, 16, 221);
            }
            else
            {
                lIconBack.BackgroundImage = (Image)Resources.ResourceManager.GetObject("back_50px");
                lBack.ForeColor = Color.FromArgb(224, 224, 224);
            }
        }
        private void BackToSignIn()
        {
            WindowState = FormWindowState.Normal;
            pnlSignIn.Controls.AddRange(new Control[] { pnlSignUp.Controls[19], pnlSignUp.Controls[20], pnlSignUp.Controls[21], pnlSignUp.Controls[22] });
            inputPhoneNumber.Location = new Point(349, 164);
            readSavedLogPass();
            pages.SelectedIndex = 0;
            RecolorBack(false);
        }
    }

}
