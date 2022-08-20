using Bunifu.UI.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Курсовая.CustomControls;
using Курсовая.DataModel;
using Курсовая.Properties;

//Добавить третий ряд картинок при регистрации
//Рассмотреть возможность добавления поиска/фильтрации по списку транзакций + убрать ограничение по кол-ву транзакций

namespace Курсовая
{
    public partial class MainForm : Form
    {
        private readonly int accID;
        private List<Account> accounts;
        private List<List<Deposit>> deposits;
        private List<List<Transaction>> transactions;
        private readonly string accPath = "Accounts.bin";
        private readonly string depPath = "Deposits.bin";
        private readonly string tranPath = "Transactions.bin";

        public MainForm(int accID, Point loc)
        {
            this.accID = accID;
            Location = loc;
            InitializeComponent();
            accounts = DataSerialiser.BinaryDeserialise(accPath) as List<Account>;
            deposits = DataSerialiser.BinaryDeserialise(depPath) as List<List<Deposit>>;
            transactions = DataSerialiser.BinaryDeserialise(tranPath) as List<List<Transaction>>;
            CheckFinishedDeps();
            CreateExchangeRates("Hryvnia");
            SetAccountInfo();
            toolTip.SetToolTip(lbtnMenu, "Show history");
            var dictImages = new Dictionary<string, Image>
            {
                { "Dollar", (Image)Resources.ResourceManager.GetObject("Dollar") },
                { "Euro", (Image)Resources.ResourceManager.GetObject("Euro") },
                { "Hryvnia", (Image)Resources.ResourceManager.GetObject("Hryvnia") }
            };
            dropDownMenu1.AddImageRange(dictImages);
            drdwCurrency1.AddImageRange(dictImages.OrderByDescending(x => x.Key.Length).ToDictionary(x => x.Key, x => x.Value));
            drdwCurrency2.AddImageRange(dictImages);
            drdw.AddImageRange(dictImages);
            panel2.BringToFront();
        }

        private void Form1_Load(object sender, EventArgs e)
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
            btnProfile.Click += (s, a) =>
            {
                Pages.SelectedIndex = 0;
            };
            btnTransfer.Click += (s, a) =>
            {
                Pages.SelectedIndex = 1;
            };
            btnDeposit.Click += (s, a) =>
            {
                Pages.SelectedIndex = 2;
                scrollBarDep.LargeChange = 65;
                scrollBarDep.BindTo(pnlDeposits);
            };
            btnExchange.Click += (s, a) =>
            {
                //UpdateDiagram();
                Pages.SelectedIndex = 3;
            };
            tbAmount.KeyPress += (s, a) =>
            {
                a.Handled = CheckCorrectInput(a, tbAmount.Text);
            };
            tbAmountDep.KeyPress += (s, a) =>
            {
                a.Handled = CheckCorrectInput(a, tbAmountDep.Text);
            };
            tbAmountEx.KeyPress += (s, a) =>
            {
                a.Handled = CheckCorrectInput(a, tbAmountEx.Text);
            };
            dropDownMenu1.elementChanged = () =>
            {
                lErrorFunds.Visible = tbAmount.Text.Length > 0 && double.Parse(tbAmount.Text) > double.Parse(page1.Controls[$"l{dropDownMenu1.CurrentElementName}"].Text);
            };
            drdw.elementChanged = () => IsFilledDeposit();
            customTextBox1.TextChange += (s, a) =>
            {
                if (customTextBox1.Text.Length == 19 && accounts.Any(x => x.cardNumber == customTextBox1.Text.Replace(" ", "")) && customTextBox1.Text.Replace(" ", "") != accounts[accID].cardNumber)
                {
                    customTextBox1.LeftIcon = (Image)Resources.ResourceManager.GetObject("ok_64px");
                    customTextBox1.LeftIcon.Tag = "Ok";
                }
                else if (customTextBox1.Text.Length == 19)
                {
                    customTextBox1.LeftIcon = (Image)Resources.ResourceManager.GetObject("bank_cards_64px");
                    customTextBox1.LeftIcon.Tag = "";
                    bunifuSnackbar1.Show(this, "Invalid card number!", type: BunifuSnackbar.MessageTypes.Error,
                        duration: 1500, position: BunifuSnackbar.Positions.MiddleRight);
                }
            };
            tbAmount.TextChanged += (s, a) =>
            {
                if (tbAmount.Text.Length > 3 && tbAmount.Text[tbAmount.Text.Length - 3] == ',') tbAmount.MaxLength = tbAmount.Text.Length;
                else
                {
                    tbAmount.MaxLength = 17;
                }
                lErrorFunds.Visible = tbAmount.Text.Length > 0 && double.Parse(tbAmount.Text) > (double)page1.Controls[$"l{dropDownMenu1.CurrentElementName}"].Tag;
            };
            btnTransferMoney.Click += (s, a) =>
            {
                if (tbAmount.Text.Length > 0 && double.Parse(tbAmount.Text) > 0 && customTextBox1.LeftIcon.Tag.ToString() == "Ok" && !lErrorFunds.Visible)
                {
                    var currency = "";
                    if (dropDownMenu1.CurrentElementName == "Dollar")
                    {
                        currency = "$";
                        accounts[accID].dollarBalance -= Math.Round(double.Parse(tbAmount.Text), 2);
                        accounts.First(x => x.cardNumber == customTextBox1.Text.Replace(" ", "")).dollarBalance += Math.Round(double.Parse(tbAmount.Text), 2);
                    }
                    else if (dropDownMenu1.CurrentElementName == "Euro")
                    {
                        currency = "€";
                        accounts[accID].euroBalance -= Math.Round(double.Parse(tbAmount.Text), 2);
                        accounts.First(x => x.cardNumber == customTextBox1.Text.Replace(" ", "")).euroBalance += Math.Round(double.Parse(tbAmount.Text), 2);
                    }
                    else
                    {
                        currency = "₴";
                        accounts[accID].hryvniaBalance -= Math.Round(double.Parse(tbAmount.Text), 2);
                        accounts.First(x => x.cardNumber == customTextBox1.Text.Replace(" ", "")).hryvniaBalance += Math.Round(double.Parse(tbAmount.Text), 2);
                    }
                    AddTransaction("Transfer", DateTime.Now, double.Parse(tbAmount.Text), currency, accID);
                    AddTransaction("Replenishment", DateTime.Now, double.Parse(tbAmount.Text), currency, accounts.First(x => x.cardNumber == customTextBox1.Text.Replace(" ", "")).Id - 1);
                    SaveData();
                    customTextBox1.LeftIcon = (Image)Resources.ResourceManager.GetObject("bank_cards_64px");
                    customTextBox1.LeftIcon.Tag = "";
                    tbAmount.Text = "";
                    customTextBox1.Text = "";
                    bunifuSnackbar1.Show(this, "Succeeded!", type: BunifuSnackbar.MessageTypes.Success,
                        duration: 1000, position: BunifuSnackbar.Positions.MiddleRight);
                }
                else
                {
                    bunifuSnackbar1.Show(this, "Data entered incorrectly!", type: BunifuSnackbar.MessageTypes.Error,
                            duration: 2000, position: BunifuSnackbar.Positions.MiddleRight);
                }
            };
            tbOutput.TextChanged += (s, a) =>
            {
                if (tbOutput.Text.Length > 3 && tbOutput.Text[tbOutput.Text.Length - 3] == ',') tbOutput.MaxLength = tbOutput.Text.Length;
                else
                {
                    tbOutput.MaxLength = 17;
                }
                if (tbOutput.Text.Length > 0 && double.Parse(tbOutput.Text) > accounts[accID].hryvniaBalance)
                {
                    bunifuSnackbar1.Show(this, "You have not enough funds!", type: BunifuSnackbar.MessageTypes.Error,
                            duration: 1000, position: BunifuSnackbar.Positions.TopCenter);
                    lbtnOutput.Visible = false;
                }
                else if (tbOutput.Text.Length == 0) lbtnOutput.Visible = false;
                else if (tbOutput.Text.Length > 0 && double.Parse(tbOutput.Text) > 0) lbtnOutput.Visible = true;
            };
            tbReplenishment.TextChanged += (s, a) =>
            {
                if (tbReplenishment.Text.Length > 3 && tbReplenishment.Text[tbReplenishment.Text.Length - 3] == ',') tbReplenishment.MaxLength = tbReplenishment.Text.Length;
                else
                {
                    tbReplenishment.MaxLength = 17;
                }
                lbtnReplenishment.Visible = tbReplenishment.Text.Length > 0 && double.Parse(tbReplenishment.Text) > 0;
            };
            lbtnOutput.Click += (s, a) =>
            {
                accounts[accID].hryvniaBalance -= Math.Round(double.Parse(tbOutput.Text), 2);
                AddTransaction("Output", DateTime.Now, double.Parse(tbOutput.Text), "₴", accID);
                SaveData();
                tbOutput.Text = "";
                lbtnOutput.Visible = false;
                bunifuSnackbar1.Show(this, "Succeeded!", type: BunifuSnackbar.MessageTypes.Success,
                        duration: 1500, position: BunifuSnackbar.Positions.TopCenter);
            };
            lbtnReplenishment.Click += (s, a) =>
            {
                accounts[accID].hryvniaBalance += Math.Round(double.Parse(tbReplenishment.Text), 2);
                AddTransaction("Replenishment", DateTime.Now, double.Parse(tbReplenishment.Text), "₴", accID);
                SaveData();
                tbReplenishment.Text = "";
                lbtnReplenishment.Visible = false;
                bunifuSnackbar1.Show(this, "Succeeded!", type: BunifuSnackbar.MessageTypes.Success,
                        duration: 1500, position: BunifuSnackbar.Positions.TopCenter);
            };
            slider.elementChanged = () =>
            {
                IsFilledDeposit();
            };
            tbAmountDep.TextChanged += (s, a) =>
            {
                if (tbAmountDep.Text.Length > 3 && tbAmountDep.Text[tbAmountDep.Text.Length - 3] == ',') tbAmountDep.MaxLength = tbAmountDep.Text.Length;
                else
                {
                    tbAmountDep.MaxLength = 15;
                }
                IsFilledDeposit();
            };
            btnCreate.Click += (s, a) =>
            {
                scrollBarDep.MinimumThumbLength = 160;
                var n = slider.CurrentElement;
                deposits[accID].Add(new Deposit(DateTime.Now, lProfit.Text.Last().ToString(), n == 1 ? 120 : n == 2 ? 600 :
                    n == 3 ? 1800 : n == 4 ? 3600 : 43200, double.Parse(tbAmountDep.Text),
                    Math.Round(double.Parse(tbAmountDep.Text) / 100 * (double)lPercents.Tag, 2)));
                if (drdw.CurrentElementName == "Dollar") accounts[accID].dollarBalance -= Math.Round(double.Parse(tbAmountDep.Text), 2);
                else if (drdw.CurrentElementName == "Euro") accounts[accID].euroBalance -= Math.Round(double.Parse(tbAmountDep.Text), 2);
                else accounts[accID].hryvniaBalance -= Math.Round(double.Parse(tbAmountDep.Text), 2);
                SaveData();
                FillDepositPanel();
                btnCreate.Visible = false;
                tbAmountDep.Text = "";
                slider.Rebuild();
                lProfit.Text = "+0$";
                lProfit.ForeColor = Color.Gray;
                lPercents.Text = "+0%";
                lPercents.ForeColor = Color.Gray;
            };
            timerDep.Tick += (s, a) => CheckFinishedDeps();
            tbAmountEx.TextChanged += (s, a) =>
            {
                if (tbAmountEx.Text.Length > 3 && tbAmountEx.Text[tbAmountEx.Text.Length - 3] == ',') tbAmountEx.MaxLength = tbAmountEx.Text.Length;
                else
                {
                    tbAmountEx.MaxLength = 17;
                }
                IsCorrectExchange();
            };
            drdwCurrency1.elementChanged = () =>
            {
                if (!drdwCurrency2.Minimized) drdwCurrency2.Collapse();
                CreateExchangeRates(drdwCurrency1.CurrentElementName);
                IsCorrectExchange();
            };
            drdwCurrency2.elementChanged = () =>
            {
                if (!drdwCurrency1.Minimized) drdwCurrency1.Collapse();
                IsCorrectExchange();
            };
            lbtnExchange.MouseEnter += (s, a) =>
            {
                lbtnExchange.Size = new Size(lbtnExchange.Width + 3, lbtnExchange.Height + 3);
            };
            lbtnExchange.MouseLeave += (s, a) =>
            {
                lbtnExchange.Size = new Size(lbtnExchange.Width - 3, lbtnExchange.Height - 3);
            };
            lbtnExchange.Click += async (s, a) =>
            {
                BeginInvoke(new Action(() =>
                {

                }));
                await Task.Run(() =>
                {
                    accounts[accID].GetType().GetProperty(drdwCurrency1.CurrentElementName.ToLower() + "Balance").
                    SetValue(accounts[accID], Math.Round(double.Parse(page1.Controls["l" + drdwCurrency1.CurrentElementName].Tag.ToString()) - double.Parse(tbAmountEx.Text), 2));
                    accounts[accID].GetType().GetProperty(drdwCurrency2.CurrentElementName.ToLower() + "Balance").
                        SetValue(accounts[accID], Math.Round(double.Parse(page1.Controls["l" + drdwCurrency2.CurrentElementName].Tag.ToString()) + (double)tbAmountEx.Tag, 2));
                    AddTransaction("Exchange", DateTime.Now, (double)tbAmountEx.Tag, lGreen.Text.Last().ToString(), accID);
                    SaveData();
                    tbAmountEx.Text = "";
                    lRed.Text = "-0₴";
                    lGreen.Text = "+0$";
                    drdwCurrency1.Collapse();
                    drdwCurrency2.Collapse();

                });
                bunifuSnackbar1.Show(this, "Succeeded!", type: BunifuSnackbar.MessageTypes.Success,
                            duration: 2000, position: BunifuSnackbar.Positions.MiddleRight);
            };
            lbtnMenu.Click += async (s, a) =>
            {
                if (pnlHistory.Visible == false)
                {
                    toolTip.SetToolTip(lbtnMenu, "Hide history");
                    bunifuTransition1.Show(pnlHistory);
                    for (int i = 0; i < 9; i++)
                    {
                        lbtnMenu.Location = new Point(lbtnMenu.Location.X - 27, lbtnMenu.Location.Y);
                        pnlCard.Location = new Point(pnlCard.Location.X - 20, pnlCard.Location.Y);
                        panel1.Location = new Point(panel1.Location.X - 25, panel1.Location.Y);
                        await Task.Delay(1);
                    }
                    scrollbarHistory.BindTo(pnlHistory);
                    scrollbarHistory.LargeChange = 200 - transactions[accID].Count;
                    lHistory.Visible = true;
                    scrollbarHistory.Visible = pnlHistory.Controls.Count > 5;
                }
                else
                {
                    lHistory.Visible = false;
                    scrollbarHistory.Visible = false;
                    toolTip.SetToolTip(lbtnMenu, "Show history");
                    bunifuTransition1.Hide(pnlHistory);
                    for (int i = 0; i < 9; i++)
                    {
                        lbtnMenu.Location = new Point(lbtnMenu.Location.X + 27, lbtnMenu.Location.Y);
                        pnlCard.Location = new Point(pnlCard.Location.X + 20, pnlCard.Location.Y);
                        panel1.Location = new Point(panel1.Location.X + 25, panel1.Location.Y);
                        await Task.Delay(1);
                    }
                }


            };
            customTextBox1.Click += (s, a) =>
            {
                if (customTextBox1.text == "") customTextBox1.Mask = "#### #### #### ####";
            };
        }
        private void AddTransaction(string type, DateTime time, double cash, string currency, int ID)
        {
            var transaction = new Transaction(type, time, Math.Round(cash, 2), currency);
            if (transactions[ID].Count == 15)
            {
                for (int i = 0; i < 14; i++)
                {
                    transactions[accID][i] = transactions[accID][i + 1];
                }
                transactions[ID][14] = transaction;
            }
            else
            {
                transactions[ID].Add(transaction);
            }
            //SaveData();
        }
        private void FillHistoryPanel()
        {
            panel2.Visible = pnlHistory.Visible;
            pnlHistory.Controls.Clear();
            for (int i = transactions[accID].Count - 1; i > -1; i--)
            {
                var transaction = transactions[accID][i];
                var t = DateTime.Now - DateTime.Parse(transaction.Time.ToString());
                var panel = new BunifuPanel
                {
                    BackgroundColor = Color.Transparent,
                    BackgroundImage = (Image)(Resources.ResourceManager.GetObject("bunifuPanel2.BackgroundImage")),
                    BackgroundImageLayout = ImageLayout.Stretch,
                    BorderColor = Color.FromArgb(140, 64, 237),
                    BorderRadius = 12,
                    BorderThickness = 1,
                    Location = new Point(8, 2 + 58 * (transactions[accID].Count - 1 - i)),
                    Name = "bunifuPanel2",
                    ShowBorders = true,
                    Size = new Size(262, 52),
                };
                var lImage = new BunifuLabel
                {
                    AutoSize = false,
                    BackgroundImage = (Image)Resources.ResourceManager.GetObject(transaction.Type),
                    BackgroundImageLayout = ImageLayout.Stretch,
                    Font = new Font("Segoe UI", 9F),
                    Location = new Point(7, 6),
                    Name = "bunifuLabel2",
                    Size = new Size(40, 40),
                };
                var lDate = new BunifuLabel
                {
                    AutoSize = false,
                    Font = new Font("Leelawadee UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0),
                    ForeColor = Color.FromArgb(137, 16, 221),
                    Location = new Point(184, 14),
                    Name = "bunifuLabel10",
                    Size = new Size(70, 21),
                    Text = (t.Days != 0 ? $"{t.Days}d" : t.Hours != 0 ? $"{t.Hours}h" : t.Minutes != 0 ? $"{t.Minutes}m" : $"{t.Seconds}s") + " ago",
                    TextAlignment = ContentAlignment.MiddleRight
                };
                var lName = new BunifuLabel
                {
                    AllowParentOverrides = false,
                    AutoEllipsis = false,
                    AutoSize = false,
                    Font = new System.Drawing.Font("Leelawadee UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0),
                    ForeColor = System.Drawing.Color.FromArgb(137, 16, 221),
                    Location = new System.Drawing.Point(53, 6),
                    Name = "bunifuLabel11",
                    Size = new System.Drawing.Size(152, 21),
                    Text = transaction.Type,
                    TextAlignment = System.Drawing.ContentAlignment.MiddleCenter,
                };
                var lCash = new BunifuLabel
                {
                    AllowParentOverrides = false,
                    AutoEllipsis = false,
                    AutoSize = false,
                    Font = new Font("Leelawadee UI", 13F),
                    ForeColor = Color.FromArgb(137, 16, 221),
                    Location = new Point(53, 26),
                    Name = "bunifuLabel12",
                    Size = new Size(152, 21),
                    Text = $"{(transaction.Type == "Output" || transaction.Type == "Transfer" ? "-" : "+")}{transaction.Cash}{transaction.Currency}",
                    TextAlignment = ContentAlignment.MiddleCenter,
                };
                panel.Controls.AddRange(new Control[] { lImage, lDate, lName, lCash });
                pnlHistory.Controls.Add(panel);
            }
            if (pnlHistory.Controls.Count > 0) pnlHistory.Controls[0].Location = new Point(8, 2);
            scrollbarHistory.Visible = pnlDeposits.Controls.Count > 5;
            panel2.Visible = false;
        }
        private void FillDepositPanel()
        {
            BeginInvoke(new Action(() =>
            {
                pnlDeposits.Controls.Clear();
                for (int i = 0; i < deposits[accID].Count; i++)
                {
                    var deposit = deposits[accID][i];
                    var date = deposit.DateOfCreation.AddSeconds(deposit.Term).ToString();
                    var panel = new BunifuPanel
                    {
                        BackgroundColor = Color.Transparent,
                        BorderColor = Color.FromArgb(137, 16, 221),
                        BorderRadius = 12,
                        BorderThickness = 1,
                        Location = new Point(9, i * 62 + 8),
                        ShowBorders = true,
                        Size = new Size(244, 56),
                    };
                    var lDate = new BunifuLabel
                    {
                        Font = new System.Drawing.Font("Leelawadee UI", 11F),
                        ForeColor = System.Drawing.Color.BlueViolet,
                        Location = new System.Drawing.Point(141, 11),
                        Size = new System.Drawing.Size(93, 21),
                        Text = date.Remove(date.Length == 19 ? 16 : 15).Remove(6, 2),
                        TextAlignment = System.Drawing.ContentAlignment.MiddleRight,
                    };
                    var lCash = new LabelEx
                    {
                        AutoSize = true,
                        Color = System.Drawing.Color.RoyalBlue,
                        Color2 = System.Drawing.Color.Magenta,
                        GradientAngle = 90f,
                        Font = new System.Drawing.Font("Leelawadee UI", 14F),
                        Location = new System.Drawing.Point(7, 8),
                        Text = $"+{deposit.Amount + deposit.Profit}{deposit.Currency}",
                        TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                    };
                    var progBar = new BunifuProgressBar
                    {
                        AllowAnimations = true,
                        Animation = 0,
                        AnimationSpeed = (int)(deposit.DateOfCreation.AddSeconds(deposit.Term) - DateTime.Now).TotalMilliseconds,
                        AnimationStep = 10,
                        BackColor = System.Drawing.Color.FromArgb(40, 36, 50),
                        BackgroundImage = (Image)Resources.ResourceManager.GetObject("bunifuProgressBar2.BackgroundImage"),
                        BorderColor = System.Drawing.Color.FromArgb(157, 16, 221),
                        BorderRadius = 9,
                        BorderThickness = 1,
                        Location = new System.Drawing.Point(8, 36),
                        Maximum = 100,
                        MaximumValue = 100,
                        Minimum = 0,
                        MinimumValue = 0,
                        ProgressBackColor = System.Drawing.Color.FromArgb(40, 36, 50),
                        ProgressColorLeft = System.Drawing.Color.RoyalBlue,
                        ProgressColorRight = System.Drawing.Color.FromArgb(155, 56, 239),
                        Size = new System.Drawing.Size(231, 14),
                        Value = (int)(((DateTime.Now - deposit.DateOfCreation).TotalSeconds + 1) / (deposit.Term / 100)),
                        ValueByTransition = 100
                    };
                    progBar.ProgressChanged += (s, a) =>
                    {
                        if (a.ProgressValue == 100) CheckFinishedDeps();
                        ///
                    };
                    panel.Controls.AddRange(new Control[] { lDate, lCash, progBar });
                    lCash.SendToBack();
                    pnlDeposits.Controls.Add(panel);
                }
                if (pnlDeposits.Controls.Count > 0) pnlDeposits.Controls[0].Location = new Point(9, 8);
                scrollBarDep.Visible = pnlDeposits.Controls.Count > 6;
            }));
        }
        private void IsFilledDeposit()
        {
            lErrorFundsDep.Visible = tbAmountDep.Text.Length > 0 && double.Parse(tbAmountDep.Text) > (double)page1.Controls[$"l{drdw.CurrentElementName}"].Tag;
            if (slider.CurrentElement != 0)
            {
                var percent = drdw.CurrentElementName == "Hryvnia" ? slider.CurrentElement + 1 : (double)(slider.CurrentElement + 1) / 2;
                lPercents.Text = "+" + percent + "%";
                lPercents.Tag = percent;
                lPercents.ForeColor = Color.Green;
            }
            if (lPercents.ForeColor == Color.Green && tbAmountDep.Text.Length > 0)
            {
                lProfit.Text = $"+{Math.Round(double.Parse(tbAmountDep.Text) / 100 * (double)lPercents.Tag, 2)}" +
                    (drdw.CurrentElementName == "Dollar" ? "$" : drdw.CurrentElementName == "Euro" ? "€" : "₴");
                lProfit.ForeColor = Color.Green;
            }
            btnCreate.Visible = lProfit.ForeColor == Color.Green && lPercents.ForeColor == Color.Green && (tbAmountDep.Text.Length > 0 && lErrorFundsDep.Visible == false && double.Parse(tbAmountDep.Text) > 0);
        }
        private async void CreateExchangeRates(string mainCurrency)
        {
            var euro = new List<double>();
            var dollar = new List<double>();
            await Task.Run(() =>
            {
                using (WebClient wc = new WebClient())
                {
                    for (int i = 6; i > 0; i--)
                    {
                        var line = wc.DownloadString($"https://api.currencyapi.com/v3/historical?apikey=X5HHcukB9UClSL7mXO6jozGgP2MCxyZPyjUPX42F&currencies=EUR%2CUSD&base_currency=UAH&date={DateTime.Now.AddDays(-i).ToString("yyyy'-'MM'-'dd")}");
                        Match match = Regex.Match(line, "\"EUR\",\"value\":(?'euro'.*?)}(.*?)\"USD\",\"value\":(?'dollar'.*?)}");
                        euro.Add(Math.Round(1 / Double.Parse(match.Groups["euro"].Value.Replace('.', ',')), 2));
                        dollar.Add(Math.Round(1 / Double.Parse(match.Groups["dollar"].Value.Replace('.', ',')), 2));
                    };
                    Match m = Regex.Match(wc.DownloadString($"https://api.currencyapi.com/v3/latest?apikey=X5HHcukB9UClSL7mXO6jozGgP2MCxyZPyjUPX42F&currencies=EUR%2CUSD&base_currency=UAH"), "\"EUR\",\"value\":(?'euro'.*?)}(.*?)\"USD\",\"value\":(?'dollar'.*?)}");
                    euro.Add(Math.Round(1 / Double.Parse(m.Groups["euro"].Value.Replace('.', ',')), 2));
                    dollar.Add(Math.Round(1 / Double.Parse(m.Groups["dollar"].Value.Replace('.', ',')), 2));
                }
            });
            var currency1 = new List<double>();
            var currency2 = new List<double>();
            if (mainCurrency == "Hryvnia")
            {
                currency1 = euro;
                currency2 = dollar;
                lCurrency1.BackgroundImage = (Image)Resources.ResourceManager.GetObject("Euro");
                lCurrency2.BackgroundImage = (Image)Resources.ResourceManager.GetObject("Dollar");
                chart1.Series[0].Name = "Euro";
                chart1.Series[1].Name = "Dollar";
            }
            else if (mainCurrency == "Dollar")
            {
                currency1 = Enumerable.Range(0, 7).Select(x => dollar[x] / euro[x]).ToList();
                currency2 = dollar.Select(x => 1 / x).ToList();
                lCurrency1.BackgroundImage = (Image)Resources.ResourceManager.GetObject("Euro");
                lCurrency2.BackgroundImage = (Image)Resources.ResourceManager.GetObject("Hryvnia");
                chart1.Series[0].Name = "Euro";
                chart1.Series[1].Name = "Hryvnia";
            }
            else
            {
                currency1 = Enumerable.Range(0, 7).Select(x => euro[x] / dollar[x]).ToList();
                currency2 = euro.Select(x => 1 / x).ToList();
                lCurrency1.BackgroundImage = (Image)Resources.ResourceManager.GetObject("Dollar");
                lCurrency2.BackgroundImage = (Image)Resources.ResourceManager.GetObject("Hryvnia");
                chart1.Series[0].Name = "Dollar ";
                chart1.Series[1].Name = "Hryvnia";
            }
            lCurrency1v.Text = "- " + Math.Round(currency1[6], 4).ToString();
            lCurrency2v.Text = "- " + Math.Round(currency2[6], 4).ToString();//$"- {currency2[6]}";
            CreateDiagram(currency1, currency2);

        }
        private void CreateDiagram(List<double> currency1, List<double> currency2)
        {
            var dates = new List<string> { "03.05", "04.05", "05.05", "06.05", "07.05", "08.05", "09.05" };
            chart1.Series[0].Points.DataBindXY(dates, currency1);
            chart1.Series[1].Points.DataBindY(currency2);
            chart1.ChartAreas[0].Axes[1].Maximum = (int)currency1.Max() + 1;
            chart1.ChartAreas[0].Axes[1].Minimum = (int)currency2.Min() - 0.5;
        }
        private bool CheckCorrectInput(KeyPressEventArgs a, string text)
        {
            if (a.KeyChar == '.') a.KeyChar = ',';
            return !(Char.IsDigit(a.KeyChar) || (a.KeyChar == ',' && !text.Contains(",")
                && text.Length > 0) || a.KeyChar == (char)Keys.Back
                || a.KeyChar == (char)Keys.Left || a.KeyChar == (char)Keys.Right);
        }
        private void SetAccountInfo()
        {
            FillHistoryPanel();
            scrollbarHistory.Visible = pnlHistory.Visible && pnlHistory.Controls.Count > 5;
            scrollbarHistory.LargeChange = 200 - transactions[accID].Count;
            var account = accounts[accID];
            pbIcon.Image = (Image)Resources.ResourceManager.GetObject(account.IconName);
            pbProfIcon.Image = pbIcon.Image;
            nickLabel.Text = account.firstName;
            lName.Text = nickLabel.Text;

            account.hryvniaBalance = Math.Round(account.hryvniaBalance, 2);
            account.dollarBalance = Math.Round(account.dollarBalance, 2);
            account.euroBalance = Math.Round(account.euroBalance, 2);
            toolTip.SetToolTip(lHryvnia, account.hryvniaBalance.ToString());
            toolTip.SetToolTip(lDollar, account.dollarBalance.ToString());
            toolTip.SetToolTip(lEuro, account.euroBalance.ToString());
            lDollar.Tag = account.dollarBalance;
            lEuro.Tag = account.euroBalance;
            lHryvnia.Tag = account.hryvniaBalance;

            lTotalBalance.Text = Math.Round(account.hryvniaBalance + account.dollarBalance * double.Parse(lCurrency1v.Text.Remove(0, 2)) + account.euroBalance * double.Parse(lCurrency2v.Text.Remove(0, 2)), 2).ToString();

            lDollar.Text = account.dollarBalance < 1 ? $"0{account.dollarBalance:.#}" : $"{(((int)account.dollarBalance).ToString().Length > 6 ? $"{account.dollarBalance / 1000000:#.##}M" : $"{account.dollarBalance:#.#}")}";
            lEuro.Text = account.euroBalance < 1 ? $"0{account.euroBalance:.#}" : $"{(((int)account.euroBalance).ToString().Length > 6 ? $"{account.euroBalance / 1000000:#.##}M" : $"{account.euroBalance:#.#}")}";
            lHryvnia.Text = account.hryvniaBalance < 1 ? $"0{account.hryvniaBalance:.#}" : $"{(((int)account.hryvniaBalance).ToString().Length > 6 ? $"{account.hryvniaBalance / 1000000:#.##}M" : $"{account.hryvniaBalance:#.#}")}";
            lCardNumber.Text = $"{long.Parse(account.cardNumber):#### #### #### ####}";
            lCardHolder.Text = $"{account.firstName.ToUpper()} {account.secondName.ToUpper()}";
            lShelfLife.Text = $"{account.cardNumber[12]}{account.cardNumber[13]}/{account.cardNumber[14]}{int.Parse(account.cardNumber[15].ToString()) + 4}";
        }
        private void CheckFinishedDeps()
        {
            var remove = new List<Deposit>();
            var transactions = new List<Transaction>();
            foreach (var deposit in deposits[accID])
            {
                if ((DateTime.Now - deposit.DateOfCreation.AddSeconds(deposit.Term)).TotalSeconds >= 0)
                {
                    remove.Add(deposit);
                    if (deposit.Currency == "$")
                    {
                        accounts[accID].dollarBalance += Math.Round(deposit.Amount + deposit.Profit, 2);
                    }
                    else if (deposit.Currency == "€")
                    {
                        accounts[accID].euroBalance += Math.Round(deposit.Amount + deposit.Profit, 2);
                    }
                    else
                    {
                        accounts[accID].hryvniaBalance += Math.Round(deposit.Amount + deposit.Profit, 2);
                    }
                    transactions.Add(new Transaction("Deposit", deposit.DateOfCreation.AddSeconds(deposit.Term), deposit.Amount + deposit.Profit, deposit.Currency));
                    snackBarDep.Show(this, $"Deposit finished! +{deposit.Amount + deposit.Profit}{deposit.Currency}", duration: 3000,
                        position: BunifuSnackbar.Positions.TopRight, type: BunifuSnackbar.MessageTypes.Success);
                }
            }
            if (remove.Count > 0)
            {
                deposits[accID].RemoveAll(x => remove.Contains(x));
                transactions.ForEach(x => AddTransaction(x.Type, x.Time, x.Cash, x.Currency, accID));
                SaveData();

            }
            FillDepositPanel();
        }
        private void IsCorrectExchange()
        {
            lErrorFundsEx.Visible = tbAmountEx.Text.Length > 0 && double.Parse(tbAmountEx.Text) > (double)page1.Controls[$"l{drdwCurrency1.CurrentElementName}"].Tag;
            if (drdwCurrency1.CurrentElementName != drdwCurrency2.CurrentElementName && lErrorFundsEx.Visible == false && tbAmountEx.Text.Length > 0 && double.Parse(tbAmountEx.Text) > 0)
            {
                var cash = double.Parse(tbAmountEx.Text);
                lRed.Text = $"-{tbAmountEx.Text}{(drdwCurrency1.CurrentElementName == "Dollar" ? "$" : drdwCurrency1.CurrentElementName == "Euro" ? "€" : "₴")}";
                if (drdwCurrency1.CurrentElementName == "Hryvnia")
                {
                    cash /= double.Parse(page4.Controls[$"lCurrency{(drdwCurrency2.CurrentElementName == "Euro" ? 1 : 2)}v"].Text.Remove(0, 2));
                }
                else if (drdwCurrency2.CurrentElementName == "Hryvnia")
                {
                    cash /= double.Parse(lCurrency2v.Text.Remove(0, 2));
                }
                else
                {
                    cash *= double.Parse(lCurrency1v.Text.Remove(0, 2));
                }
                lGreen.Text = $"+{Math.Round(cash, 2)}{(drdwCurrency2.CurrentElementName == "Dollar" ? "$" : drdwCurrency2.CurrentElementName == "Euro" ? "€" : "₴")}";
                tbAmountEx.Tag = cash;
                lbtnExchange.Visible = true;
            }
            else
            {
                lRed.Text = $"-0{(drdwCurrency1.CurrentElementName == "Dollar" ? "$" : drdwCurrency1.CurrentElementName == "Euro" ? "€" : "₴")}";
                lGreen.Text = $"-0{(drdwCurrency2.CurrentElementName == "Dollar" ? "$" : drdwCurrency2.CurrentElementName == "Euro" ? "€" : "₴")}";
                lbtnExchange.Visible = false;
            }
        }
        private void SaveData(bool read = true)
        {
            BeginInvoke(new Action(() =>
            {
                DataSerialiser.BinarySerialise(accounts, accPath);
                DataSerialiser.BinarySerialise(deposits, depPath);
                DataSerialiser.BinarySerialise(transactions, tranPath);
                if (read)
                {
                    accounts = DataSerialiser.BinaryDeserialise(accPath) as List<Account>;
                    deposits = DataSerialiser.BinaryDeserialise(depPath) as List<List<Deposit>>;
                    transactions = DataSerialiser.BinaryDeserialise(tranPath) as List<List<Transaction>>;
                    SetAccountInfo();
                }
            }));

        }
        private void tbOutput_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CheckCorrectInput(e, tbOutput.Text);
        }
        private void tbReplenishment_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CheckCorrectInput(e, tbReplenishment.Text);
        }
    }
}
