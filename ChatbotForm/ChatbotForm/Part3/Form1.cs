using System.Media;
using System.IO;
using ChatbotForm.Core.Models;
using ChatbotForm.Core.Services;
using POE_ST10434048.Part_2;

namespace ChatbotForm.Part3;

public partial class Form1 : Form
{
    private string _name = Part1.Part1._name;
    private TextBox txtInput;
    private Button btnSend;
    private Button btnHelp;
    private RichTextBox rtbOutput;
    private Panel pnlHeader;
    private Label lblTitle;
    private Label lblStatus;
    private ProgressBar progressBar;
    private System.Windows.Forms.Timer taskCheckTimer;
    private string _pendingTaskName = null; // Track when waiting for time specification
    
    // Enhanced color scheme with modern gradients
    private readonly Color PrimaryColor = Color.FromArgb(0, 150, 255); // Modern blue
    private readonly Color SecondaryColor = Color.FromArgb(0, 200, 100); // Modern green
    private readonly Color AccentColor = Color.FromArgb(255, 193, 7); // Modern yellow
    private readonly Color ErrorColor = Color.FromArgb(220, 53, 69); // Modern red
    private readonly Color SuccessColor = Color.FromArgb(40, 167, 69); // Modern green
    private readonly Color BackgroundColor = Color.FromArgb(18, 18, 18); // Darker background
    private readonly Color SurfaceColor = Color.FromArgb(28, 28, 28); // Slightly lighter surface
    private readonly Color TextColor = Color.FromArgb(240, 240, 240); // Off-white text
    private readonly Color MutedTextColor = Color.FromArgb(180, 180, 180); // Muted text
    
    // Quiz state variables
    private bool quizInProgress = false;
    private List<QuizQuestion> quizQuestions;
    private int quizCurrentIndex = 0;
    private List<int> quizUserAnswers;
    private int quizScore = 0;
    
    public Form1()
    {
        InitializeComponent();
        
        // Set custom icon
        try
        {
            // Try to load chat icon from pictures folder
            string chatIconPath = Path.Combine(Application.StartupPath, "pictures", "chat-icon.ico");
            if (File.Exists(chatIconPath))
            {
                this.Icon = new Icon(chatIconPath);
            }
        }
        catch
        {
            // If icon loading fails, use default
        }
        
        Part1.Part1.InitializeKeywords();
        SetupUI();
        PlayWelcomeAudio();
        ShowWelcomeMessage();
        
        // Start a new chat session
        ChatLogger.StartNewSession();
        
        // Check for overdue tasks on startup
        TaskManager.CheckAndNotifyOverdueTasks();
        
        // Set up timer to check for overdue tasks every 5 minutes
        taskCheckTimer = new System.Windows.Forms.Timer();
        taskCheckTimer.Interval = 5 * 60 * 1000; // 5 minutes
        taskCheckTimer.Tick += TaskCheckTimer_Tick;
        taskCheckTimer.Start();
    }
    
    private void SetupUI()
    {
        // Set form properties
        this.BackColor = BackgroundColor;
        this.ForeColor = TextColor;
        this.Font = new Font("Segoe UI", 9);
        
        // Header Panel with gradient effect
        pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 100,
            BackColor = SurfaceColor,
            Padding = new Padding(20, 15, 20, 15)
        };
        
        // Add subtle border to header
        pnlHeader.Paint += (sender, e) =>
        {
            using (Pen pen = new Pen(Color.FromArgb(60, 60, 60), 1))
            {
                e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
            }
        };

        // Title Label with modern styling
        lblTitle = new Label
        {
            Text = "🛡️ Cybersecurity Awareness Bot",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = PrimaryColor,
            AutoSize = true,
            Location = new Point(20, 15)
        };

        // Status Label with modern styling
        lblStatus = new Label
        {
            Text = "Ready to help you learn about cybersecurity!",
            Font = new Font("Segoe UI", 11),
            ForeColor = MutedTextColor,
            AutoSize = true,
            Location = new Point(20, 50)
        };

        // Enhanced Progress Bar
        progressBar = new ProgressBar
        {
            Dock = DockStyle.Bottom,
            Height = 4,
            Style = ProgressBarStyle.Continuous,
            ForeColor = PrimaryColor,
            BackColor = Color.FromArgb(40, 40, 40),
            Visible = false
        };

        // Enhanced Output RichTextBox
        rtbOutput = new RichTextBox
        {
            Dock = DockStyle.Fill,
            BackColor = BackgroundColor,
            ForeColor = TextColor,
            Font = new Font("Segoe UI", 12),
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            Margin = new Padding(15),
            SelectionBackColor = Color.FromArgb(60, 60, 60),
            SelectionColor = TextColor
        };

        // Input Panel with modern styling
        Panel pnlInput = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 90,
            BackColor = SurfaceColor,
            Padding = new Padding(15)
        };
        
        // Add subtle border to input panel
        pnlInput.Paint += (sender, e) =>
        {
            using (Pen pen = new Pen(Color.FromArgb(60, 60, 60), 1))
            {
                e.Graphics.DrawLine(pen, 0, 0, pnlInput.Width, 0);
            }
        };

        // Enhanced Send Button with modern styling
        btnSend = new Button
        {
            Text = "Send",
            Width = 80,
            Height = 35,
            BackColor = PrimaryColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Right | AnchorStyles.Top,
            Location = new Point(pnlInput.Width - 100, 15)
        };
        btnSend.FlatAppearance.BorderSize = 0;
        btnSend.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 130, 220);
        btnSend.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 110, 200);
        btnSend.Click += BtnSend_Click;

        // Floating Help Button
        btnHelp = new Button
        {
            Text = "?",
            Width = 35,
            Height = 35,
            BackColor = AccentColor,
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnHelp.FlatAppearance.BorderSize = 0;
        btnHelp.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 213, 77);
        btnHelp.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 193, 7);
        btnHelp.Click += async (s, e) => { await HandleHelpRequest(); };
        
        // Enhanced Input TextBox with rounded corners
        txtInput = new TextBox
        {
            BackColor = Color.FromArgb(45, 45, 45),
            ForeColor = TextColor,
            Font = new Font("Segoe UI", 14),
            BorderStyle = BorderStyle.FixedSingle,
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
            Location = new Point(15, 15),
            Width = pnlInput.Width - 120,
            Height = 35
        };
        txtInput.KeyPress += TxtInput_KeyPress;
        
        // Add placeholder text effect
        txtInput.Text = "Type your message here...";
        txtInput.ForeColor = MutedTextColor;
        txtInput.Enter += (sender, e) =>
        {
            if (txtInput.Text == "Type your message here...")
            {
                txtInput.Text = "";
                txtInput.ForeColor = TextColor;
            }
        };
        txtInput.Leave += (sender, e) =>
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                txtInput.Text = "Type your message here...";
                txtInput.ForeColor = MutedTextColor;
            }
        };

        // Add controls to form
        pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblStatus });
        pnlInput.Controls.AddRange(new Control[] { txtInput, btnSend });
        
        this.Controls.AddRange(new Control[] { 
            rtbOutput, 
            pnlInput, 
            pnlHeader, 
            progressBar, 
            btnHelp
        });

        // Position the help button in the top-right corner of the chat area
        this.Load += (s, e) => {
            btnHelp.Location = new Point(this.ClientSize.Width - btnHelp.Width - 20, pnlHeader.Height + 20);
            btnHelp.BringToFront();
            
            // Auto-focus the input field
            txtInput.Focus();
        };
    }

    private void ShowWelcomeMessage()
    {
        AppendColoredText("🛡️ Welcome to the Cybersecurity Awareness Bot!\n\n", PrimaryColor, true);
        DisplayCenteredAsciiArt();
        AppendColoredText("What should I call you?\n", AccentColor);
        lblStatus.Text = "Please enter your name to get started...";
    }

    private void DisplayCenteredAsciiArt()
    {
        // Split the ASCII art into lines and display each line centered
        string[] lines = Part1.Part1.asciiArt.Split('\n');
        
        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                // Use RichTextBox's built-in centering
                rtbOutput.SelectionStart = rtbOutput.TextLength;
                rtbOutput.SelectionLength = 0;
                rtbOutput.SelectionFont = new Font(rtbOutput.Font, FontStyle.Bold);
                rtbOutput.SelectionColor = PrimaryColor;
                rtbOutput.SelectionAlignment = HorizontalAlignment.Center;
                rtbOutput.AppendText(line + "\n");
                rtbOutput.SelectionAlignment = HorizontalAlignment.Left;
            }
        }
        rtbOutput.ScrollToCaret();
    }

    private void PlayWelcomeAudio()
    {
        try
        {
            // Play welcome sound if file exists
            string soundPath = AppDomain.CurrentDomain.BaseDirectory + @"Sounds\Welcome.wav";
            if (System.IO.File.Exists(soundPath))
            {
                SoundPlayer player = new SoundPlayer(soundPath);
                player.Play();
            }
        }
        catch
        {
            // Ignore audio errors
        }
    }

    private void TxtInput_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            e.Handled = true;
            ProcessInput();
        }
    }

    private void BtnSend_Click(object sender, EventArgs e)
    {
        ProcessInput();
    }

    private async void ProcessInput()
    {
        string input = txtInput.Text.Trim();
        if (string.IsNullOrEmpty(input) || input == "Type your message here...") return;

        // Display user input with enhanced formatting
        AppendMessage("You", input, Color.LightBlue, TextColor);
        txtInput.Clear();
        txtInput.ForeColor = TextColor;

        // Show thinking animation
        await ShowThinkingAnimation();

        // Process the input
        if (string.IsNullOrEmpty(_name))
        {
            await HandleNameInput(input);
        }
        else if (_pendingTaskName != null)
        {
            await HandleTimeSpecification(input);
        }
        else if (quizInProgress)
        {
            await HandleQuizAnswer(input);
        }
        else
        {
            await ProcessResponse(input);
        }
    }

    private async Task HandleNameInput(string input)
    {
        _name = input.Trim();
        Part1.Part1._name = _name;
        
        // Store name in user memory
        UserMemory.StoreName(_name);
        
        // Friendly greeting
        string setupMsg = $"Welcome, {_name}! I'm here to help you learn about cybersecurity and manage your security tasks.";
        AppendMessage("Bot", setupMsg, PrimaryColor, TextColor);
        ChatLogger.LogBotMessage(setupMsg);

        // Show features
        AppendColoredText("\n You can create a task, ask about a topic, take a quiz, or see past chat logs.\n", AccentColor, true);

        AppendColoredText("\nType 'help' at any time to learn more about what I can do!\n", PrimaryColor);
        
    }

    private async Task ProcessResponse(string input)
    {
        // Log user message
        ChatLogger.LogUserMessage(input);
        
        string inputLower = input.ToLower();
        
        // Handle exit commands
        if (inputLower.Contains("exit") || inputLower.Contains("bye") || inputLower.Contains("goodbye"))
        {
            string exitMsg = "Thank you for using the Cybersecurity Awareness Bot. Stay safe online!";
            AppendMessage("Bot", exitMsg, PrimaryColor, TextColor);
            ChatLogger.LogBotMessage(exitMsg);
            await Task.Delay(2000);
            Application.Exit();
            return;
        }

        // Handle help request
        if (inputLower.Contains("help"))
        {
            await HandleHelpRequest();
            return;
        }

        // Handle task management commands
        if (await HandleTaskCommands(input, inputLower))
        {
            return;
        }

        // Handle chat logging commands
        if (await HandleChatCommands(input, inputLower))
        {
            return;
        }

        // Handle activity logging commands
        if (inputLower.Contains("activity"))
        {
            await HandleViewActivityLog(inputLower);
            return;
        }

        // Handle quiz commands
        if (inputLower.Contains("quiz"))
        {
            await HandleStartQuiz(inputLower);
            return;
        }

        // Check for tip requests FIRST - look for tip keywords and determine topic
        if (inputLower.Contains("tip") || inputLower.Contains("tips"))
        {
            // Extract topic from the tip request (e.g., "tip on vpn", "vpn tips", "password tip")
            string tipTopic = ExtractTopicFromTipRequest(input);
            
            if (!string.IsNullOrEmpty(tipTopic) && tipTopic != "general")
            {
                // User specified a topic in the tip request
                string directTip = POE_ST10434048.Part_2.Part2.DisplayTips(tipTopic);
                AppendMessage("Bot", $"💡 Tip for {tipTopic}: {directTip}", PrimaryColor, TextColor);
                ChatLogger.LogBotMessage($"Tip displayed for {tipTopic}");
                return;
            }
            else
            {
                // No topic specified, try to use LastTopic
                string lastTopic = POE_ST10434048.Part_2.Part2.LastTopic;
                
                if (!string.IsNullOrEmpty(lastTopic))
                {
                    string directTip = POE_ST10434048.Part_2.Part2.DisplayTips(lastTopic);
                    AppendMessage("Bot", $"💡 Tip for {lastTopic}: {directTip}", PrimaryColor, TextColor);
                    ChatLogger.LogBotMessage($"Tip displayed for {lastTopic}");
                    return;
                }
                else
                {
                    AppendMessage("Bot", "Please ask about a topic first (like 'tell me about VPN'), then ask for tips!", AccentColor, TextColor);
                    ChatLogger.LogBotMessage("Tip requested but no topic available");
                    return;
                }
            }
        }

        // Check if user is asking about a new topic (not a tip request)
        string newTopic = ExtractTopicFromInput(input);
        if (!string.IsNullOrEmpty(newTopic) && newTopic != "general")
        {
            // Clear the last topic since user is asking about a new topic
            POE_ST10434048.Part_2.Part2.LastTopic = null;
            await HandleTopicDiscussion(newTopic);
            return;
        }

        // Handle follow-up questions (only if not asking for a new topic or tips)
        string followUpResponse = POE_ST10434048.Part_2.Part2.HandleFollowUp(input);
        if (!string.IsNullOrEmpty(followUpResponse))
        {
            AppendMessage("Bot", followUpResponse, PrimaryColor, TextColor);
            ChatLogger.LogBotMessage(followUpResponse);
            return;
        }

        // Handle general conversation
        switch (inputLower)
        {
            case "how are you":
                string howAreYouMsg = "I'm doing great, thanks for asking! I'm here to help you learn about cybersecurity.";
                AppendMessage("Bot", howAreYouMsg, PrimaryColor, TextColor);
                ChatLogger.LogBotMessage(howAreYouMsg);
                break;

            case "purpose":
                string purposeMsg = "My purpose is to help you learn about cybersecurity topics in a conversational way. I can answer questions about various aspects of staying safe online.";
                AppendMessage("Bot", purposeMsg, PrimaryColor, TextColor);
                ChatLogger.LogBotMessage(purposeMsg);
                break;

            case "ask about":
                string askAboutMsg = "You can ask me about: password security, phishing attacks, safe browsing habits, malware protection, social engineering, two-factor authentication, VPNs, data breaches, firewalls, and encryption.";
                AppendMessage("Bot", askAboutMsg, PrimaryColor, TextColor);
                ChatLogger.LogBotMessage(askAboutMsg);
                break;

            default:
                // Check if it's a general cybersecurity question
                if (ContainsKeywords(inputLower, "cybersecurity") || ContainsKeywords(inputLower, "security"))
                {
                    string cybersecurityMsg = "I can help you with various cybersecurity topics. Try asking about passwords, phishing, malware, VPNs, firewalls, encryption, two-factor authentication, social engineering, data breaches, or safe browsing.";
                    AppendMessage("Bot", cybersecurityMsg, PrimaryColor, TextColor);
                    ChatLogger.LogBotMessage(cybersecurityMsg);
                }
                else
                {
                    string defaultMsg = "I'm not sure I understand. Try asking about cybersecurity topics like passwords, phishing, malware, or type 'help' to see what I can help you with.";
                    AppendMessage("Bot", defaultMsg, PrimaryColor, TextColor);
                    ChatLogger.LogBotMessage(defaultMsg);
                }
                break;
        }
    }

    private async Task<bool> HandleTaskCommands(string input, string inputLower)
    {
        // Check if we're waiting for a time specification
        if (_pendingTaskName != null)
        {
            await HandleTimeSpecification(input);
            return true;
        }

        // Handle task creation with NLP
        if (inputLower.Contains("remind me") || inputLower.Contains("create task") || inputLower.Contains("add task"))
        {
            await HandleCreateTaskWithNLP(input);
            return true;
        }

        // Handle remove all tasks
        if (inputLower.Contains("remove all tasks") || inputLower.Contains("delete all tasks") || inputLower.Contains("clear all tasks"))
        {
            await HandleRemoveAllTasks();
            return true;
        }

        // Handle task deletion (specific task)
        if ((inputLower.Contains("delete task") || inputLower.Contains("remove task")) &&
            !(inputLower.Contains("delete all tasks") || inputLower.Contains("remove all tasks") || inputLower.Contains("clear all tasks")))
        {
            await HandleDeleteTask(input);
            return true;
        }

        // Handle task viewing
        if (inputLower.Contains("show tasks") || inputLower.Contains("view tasks") || inputLower.Contains("list tasks") || 
            inputLower.Contains("see all tasks") || inputLower.Contains("all tasks") || inputLower.Contains("view all tasks"))
        {
            await HandleViewTasks(inputLower);
            return true;
        }

        // Handle task completion
        if (inputLower.Contains("complete task") || inputLower.Contains("finish task") || inputLower.Contains("mark complete"))
        {
            await HandleCompleteTask(input);
            return true;
        }

        // Handle task summary
        if (inputLower.Contains("task summary") || inputLower.Contains("tasks summary"))
        {
            await HandleTaskSummary();
            return true;
        }

        // Handle task help
        if (inputLower.Contains("task help") || inputLower.Contains("tasks help"))
        {
            await HandleTaskHelp();
            return true;
        }

        return false;
    }

    private async Task HandleCreateTaskWithNLP(string input)
    {
        string taskName = NLPProcessor.ExtractTaskName(input);
        string timeInfo = NLPProcessor.ExtractTimeInfo(input);

        if (string.IsNullOrWhiteSpace(taskName))
        {
            string errorMsg = "I couldn't understand what task you want to create. Try saying something like 'remind me to update my password' or 'create task to enable 2FA'.";
            AppendMessage("Bot", errorMsg, ErrorColor, TextColor);
            ChatLogger.LogBotMessage(errorMsg);
            return;
        }

        // If time info is included in the same input, create task
        if (!string.IsNullOrWhiteSpace(timeInfo))
        {
            await CreateTaskWithTime(taskName, timeInfo);
        }
        else
        {
            // Ask for time specification
            _pendingTaskName = taskName;
            string timeMsg = $"Great! I'll create a task for '{taskName}'. When would you like to be reminded? (e.g., 'tomorrow', '3pm', 'in 2 hours')";
            AppendMessage("Bot", timeMsg, PrimaryColor, TextColor);
            ChatLogger.LogBotMessage(timeMsg);
        }
    }

    private async Task CreateTaskWithTime(string taskName, string timeInfo)
    {
        try
        {
            DateTime reminderTime = TaskManager.ParseTimeInput(timeInfo);
            TaskManager.CreateTask(taskName, $"Reminder to {taskName}", reminderTime);
            
            string successMsg = $"✅ Task created: '{taskName}' - Reminder set for {reminderTime:g}";
            AppendMessage("Bot", successMsg, SecondaryColor, TextColor);
            ChatLogger.LogBotMessage(successMsg, "task");
            ActivityLogger.LogTaskCreated(taskName, _name);
            
            lblStatus.Text = $"Task created: {taskName}";
        }
        catch (Exception ex)
        {
            string errorMsg = $"❌ Error creating task: {ex.Message}";
            AppendMessage("Bot", errorMsg, ErrorColor, TextColor);
            ChatLogger.LogBotMessage(errorMsg, "task");
        }
    }

    private async Task HandleHelpRequest()
    {
        AppendColoredText("🤖 Cybersecurity Bot Help\n\n", PrimaryColor, true);
        AppendColoredText("Here's what I can help you with:\n\n", TextColor);
        AppendColoredText("📋 Task Management\n", SecondaryColor, true);
        AppendColoredText("Manage your security-related tasks and reminders. Type 'task help' for details.\n\n", TextColor);
        AppendColoredText("💬 Chat Logging\n", SecondaryColor, true);
        AppendColoredText("View and manage your chat history. Type 'chat help' for details.\n\n", TextColor);
        AppendColoredText("📊 Activity Logging\n", SecondaryColor, true);
        AppendColoredText("Track your activity and see summaries. Type 'activity help' for details.\n\n", TextColor);
        AppendColoredText("🧠 Interactive Quiz\n", SecondaryColor, true);
        AppendColoredText("Test your cybersecurity knowledge. Type 'quiz help' for details.\n\n", TextColor);
        AppendColoredText("🔒 Cybersecurity Topics\n", SecondaryColor, true);
        AppendColoredText("Ask me about passwords, phishing, malware, VPNs, firewalls, encryption, and more!\n\n", TextColor);
        AppendColoredText("Type the feature name + 'help' (e.g., 'task help') to see detailed commands for that feature.\n", AccentColor);
        ChatLogger.LogBotMessage("Help information displayed");
        lblStatus.Text = "Help displayed!";
    }

//part 2 implimentation
    private async Task HandleTopicDiscussion(string topic)
    {
        // Set the topic for follow-up questions
        Part2.SetTopic(topic);
        
        switch (topic.ToLower())
        {
            case "password":
                await ShowPasswordInfo();
                ActivityLogger.LogTopicDiscussed("password", _name);
                break;
                
            case "phishing":
                await ShowPhishingInfo();
                ActivityLogger.LogTopicDiscussed("phishing", _name);
                break;
                
            case "malware":
                await ShowMalwareInfo();
                ActivityLogger.LogTopicDiscussed("malware", _name);
                break;
                
            case "vpn":
                await ShowVpnInfo();
                ActivityLogger.LogTopicDiscussed("vpn", _name);
                break;
                
            case "firewall":
                await ShowFirewallInfo();
                ActivityLogger.LogTopicDiscussed("firewall", _name);
                break;
                
            case "encryption":
                await ShowEncryptionInfo();
                ActivityLogger.LogTopicDiscussed("encryption", _name);
                break;
                
            case "two factor":
            case "2fa":
                await Show2FAInfo();
                ActivityLogger.LogTopicDiscussed("two factor", _name);
                break;
                
            case "social engineering":
                await ShowSocialEngineeringInfo();
                ActivityLogger.LogTopicDiscussed("social engineering", _name);
                break;
                
            case "data breach":
                await ShowDataBreachInfo();
                ActivityLogger.LogTopicDiscussed("data breach", _name);
                break;
                
            case "safe browsing":
                await ShowSafeBrowsingInfo();
                ActivityLogger.LogTopicDiscussed("safe browsing", _name);
                break;
                
            default:
                string topicMsg = $"I can help you learn about {topic}. What specific aspect would you like to know more about?";
                AppendMessage("Bot", topicMsg, PrimaryColor, TextColor);
                ChatLogger.LogBotMessage(topicMsg, topic);
                break;
        }
    }
    //part 2 implimentation
    // Add the missing topic info methods
    private async Task ShowFirewallInfo()
    {
        AppendColoredText("🛡️ Firewall Protection\n\n", PrimaryColor, true);
        AppendColoredText("A firewall is a network security device that monitors and filters incoming and outgoing network traffic based on security policies.\n\n", TextColor);
        AppendColoredText("Key Points:\n", SecondaryColor);
        AppendColoredText("• Personal firewalls protect individual devices\n", TextColor);
        AppendColoredText("• Network firewalls protect entire networks\n", TextColor);
        AppendColoredText("• Always keep your firewall enabled and properly configured\n", TextColor);
        AppendColoredText("• Firewalls block unauthorized access while permitting legitimate communications\n\n", TextColor);
        
        ChatLogger.LogBotMessage("Firewall information displayed", "firewall");
        ActivityLogger.LogTopicDiscussed("firewall", _name);
    }
    //part 2 implimentation
    private async Task ShowEncryptionInfo()
    {
        AppendColoredText("🔐 Encryption\n\n", PrimaryColor, true);
        AppendColoredText("Encryption converts data into a coded format that can only be read with the correct decryption key.\n\n", TextColor);
        AppendColoredText("Key Points:\n", SecondaryColor);
        AppendColoredText("• Protects data during storage (at rest) and transmission (in transit)\n", TextColor);
        AppendColoredText("• Use encrypted communication apps\n", TextColor);
        AppendColoredText("• Enable device encryption\n", TextColor);
        AppendColoredText("• Look for HTTPS in website URLs for encrypted connections\n\n", TextColor);
        
        ChatLogger.LogBotMessage("Encryption information displayed", "encryption");
        ActivityLogger.LogTopicDiscussed("encryption", _name);
    }
    //part 2 implimentation
    private async Task Show2FAInfo()
    {
        AppendColoredText("🔐 Two-Factor Authentication (2FA)\n\n", PrimaryColor, true);
        AppendColoredText("Two-factor authentication adds an extra layer of security beyond just a password.\n\n", TextColor);
        AppendColoredText("Key Points:\n", SecondaryColor);
        AppendColoredText("• Requires two factors: something you know (password) + something you have (phone)\n", TextColor);
        AppendColoredText("• Enable 2FA on all important accounts\n", TextColor);
        AppendColoredText("• Especially important for email, banking, and social media\n", TextColor);
        AppendColoredText("• Significantly improves account security\n\n", TextColor);
        
        ChatLogger.LogBotMessage("2FA information displayed", "two factor");
        ActivityLogger.LogTopicDiscussed("two factor", _name);
    }
    //part 2 implimentation
    private async Task ShowSocialEngineeringInfo()
    {
        AppendColoredText("🧠 Social Engineering\n\n", PrimaryColor, true);
        AppendColoredText("Social engineering attacks manipulate people into breaking security procedures or revealing confidential information.\n\n", TextColor);
        AppendColoredText("Common Tactics:\n", SecondaryColor);
        AppendColoredText("• Pretexting (creating fabricated scenarios)\n", TextColor);
        AppendColoredText("• Baiting with attractive offers\n", TextColor);
        AppendColoredText("• Quid pro quo offers\n", TextColor);
        AppendColoredText("• Tailgating (following into restricted areas)\n\n", TextColor);
        AppendColoredText("Protection:\n", SecondaryColor);
        AppendColoredText("• Always verify identities before sharing sensitive information\n", TextColor);
        AppendColoredText("• Be skeptical of unusual requests\n", TextColor);
        AppendColoredText("• Question too-good-to-be-true offers\n\n", TextColor);
        
        ChatLogger.LogBotMessage("Social engineering information displayed", "social engineering");
        ActivityLogger.LogTopicDiscussed("social engineering", _name);
    }
    //part 2 implimentation
    private async Task ShowDataBreachInfo()
    {
        AppendColoredText("💥 Data Breaches\n\n", PrimaryColor, true);
        AppendColoredText("A data breach occurs when unauthorized individuals gain access to sensitive, protected, or confidential data.\n\n", TextColor);
        AppendColoredText("If Your Data is Breached:\n", SecondaryColor);
        AppendColoredText("• Change passwords immediately\n", TextColor);
        AppendColoredText("• Monitor accounts for suspicious activity\n", TextColor);
        AppendColoredText("• Consider credit freezes\n", TextColor);
        AppendColoredText("• Be alert for phishing attempts\n\n", TextColor);
        AppendColoredText("Check Your Exposure:\n", SecondaryColor);
        AppendColoredText("• Use services like HaveIBeenPwned.com\n", TextColor);
        AppendColoredText("• Check if your email has been involved in known breaches\n\n", TextColor);
        
        ChatLogger.LogBotMessage("Data breach information displayed", "data breach");
        ActivityLogger.LogTopicDiscussed("data breach", _name);
    }
    //part 2 implimentation
    private async Task ShowSafeBrowsingInfo()
    {
        AppendColoredText("🌐 Safe Browsing\n\n", PrimaryColor, true);
        AppendColoredText("Safe browsing refers to technologies that identify and warn users about malicious websites containing phishing or malware threats.\n\n", TextColor);
        AppendColoredText("Key Points:\n", SecondaryColor);
        AppendColoredText("• Google Safe Browsing identifies harmful sites\n", TextColor);
        AppendColoredText("• Warns users before they can cause damage\n", TextColor);
        AppendColoredText("• Helps protect against phishing and malware\n", TextColor);
        AppendColoredText("• Always check URLs before clicking\n\n", TextColor);
        
        ChatLogger.LogBotMessage("Safe browsing information displayed", "safe browsing");
        ActivityLogger.LogTopicDiscussed("safe browsing", _name);
    }

    private async Task HandleViewTasks(string inputLower)
    {
        AppendColoredText("📋 Loading your tasks...\n", PrimaryColor);
        await ShowProgressBar();

        List<SecurityTask> tasks;
        string title;

        if (inputLower.Contains("overdue"))
        {
            tasks = TaskManager.GetOverdueTasks();
            title = "Overdue Tasks";
        }
        else if (inputLower.Contains("pending") || inputLower.Contains("incomplete"))
        {
            tasks = TaskManager.GetPendingTasks();
            title = "Pending Tasks";
        }
        else if (inputLower.Contains("completed"))
        {
            var allTasks = TaskManager.GetAllTasks();
            tasks = allTasks.Where(t => t.IsCompleted).ToList();
            title = "Completed Tasks";
        }
        else
        {
            tasks = TaskManager.GetAllTasks();
            title = "All Tasks";
        }

        string taskList = TaskManager.FormatTaskList(tasks, title);
        AppendColoredText(taskList + "\n\n", TextColor);
        ChatLogger.LogBotMessage($"Displayed {title}: {tasks.Count} tasks found", "task");

        lblStatus.Text = "Tasks displayed successfully!";
    }

    private async Task HandleCompleteTask(string input)
    {
        // Extract task name from input
        string taskName = ExtractTaskName(input);
        
        if (string.IsNullOrEmpty(taskName))
        {
            string errorMsg = "Please specify which task to complete (e.g., 'complete task change password').";
            AppendMessage("Bot", errorMsg, ErrorColor, TextColor);
            ChatLogger.LogBotMessage(errorMsg, "task");
            return;
        }

        AppendColoredText("✅ Marking task as complete...\n", PrimaryColor);
        await ShowProgressBar();

        bool success = TaskManager.MarkTaskAsCompleted(taskName);
        if (success)
        {
            string successMsg = $"Task '{taskName}' marked as completed!";
            AppendMessage("Bot", successMsg, SecondaryColor, TextColor);
            ChatLogger.LogBotMessage(successMsg, "task");
            lblStatus.Text = $"Task '{taskName}' completed!";
        }
        else
        {
            string errorMsg = $"Task '{taskName}' not found or already completed.";
            AppendMessage("Bot", errorMsg, ErrorColor, TextColor);
            ChatLogger.LogBotMessage(errorMsg, "task");
            lblStatus.Text = "Task not found";
        }
    }

    private async Task HandleDeleteTask(string input)
    {
        try
        {
            // Extract task name from input
            string taskName = ExtractTaskName(input);
            
            if (string.IsNullOrWhiteSpace(taskName))
            {
                string errorMsg = "I couldn't understand which task you want to delete. Try saying something like 'delete task change password' or 'remove task update 2FA'.";
                AppendMessage("Bot", errorMsg, ErrorColor, TextColor);
                ChatLogger.LogBotMessage(errorMsg, "task");
                return;
            }

            bool deleted = TaskManager.DeleteTask(taskName);
            
            if (deleted)
            {
                string successMsg = $"✅ Task deleted: '{taskName}'";
                AppendMessage("Bot", successMsg, SuccessColor, TextColor);
                ChatLogger.LogBotMessage(successMsg, "task");
                lblStatus.Text = $"Task deleted: {taskName}";
            }
            else
            {
                string errorMsg = $"❌ Task '{taskName}' not found. Use 'show tasks' to see your current tasks.";
                AppendMessage("Bot", errorMsg, ErrorColor, TextColor);
                ChatLogger.LogBotMessage(errorMsg, "task");
            }
        }
        catch (Exception ex)
        {
            string errorMsg = $"❌ Error deleting task: {ex.Message}";
            AppendMessage("Bot", errorMsg, ErrorColor, TextColor);
            ChatLogger.LogBotMessage(errorMsg, "task");
        }
    }

    private async Task HandleRemoveAllTasks()
    {
        try
        {
            var currentTasks = TaskManager.GetAllTasks();
            
            if (currentTasks.Count == 0)
            {
                string msg = "📋 No tasks to remove. Your task list is already empty.";
                AppendMessage("Bot", msg, AccentColor, TextColor);
                ChatLogger.LogBotMessage(msg, "task");
                return;
            }

            bool removed = TaskManager.RemoveAllTasks();
            
            if (removed)
            {
                string successMsg = $"🗑️ Removed all {currentTasks.Count} tasks successfully.";
                AppendMessage("Bot", successMsg, SuccessColor, TextColor);
                ChatLogger.LogBotMessage(successMsg, "task");
                lblStatus.Text = $"Removed all {currentTasks.Count} tasks";
            }
            else
            {
                string errorMsg = "❌ Error removing tasks. Please try again.";
                AppendMessage("Bot", errorMsg, ErrorColor, TextColor);
                ChatLogger.LogBotMessage(errorMsg, "task");
            }
        }
        catch (Exception ex)
        {
            string errorMsg = $"❌ Error removing all tasks: {ex.Message}";
            AppendMessage("Bot", errorMsg, ErrorColor, TextColor);
            ChatLogger.LogBotMessage(errorMsg, "task");
        }
    }

    private async Task HandleTaskSummary()
    {
        AppendColoredText("📊 Generating task summary...\n", PrimaryColor);
        await ShowProgressBar();

        string summary = TaskManager.GetTasksSummary();
        AppendColoredText(summary + "\n\n", TextColor);
        ChatLogger.LogBotMessage("Task summary displayed", "task");
        
        lblStatus.Text = "Task summary displayed!";
    }

    private async Task HandleViewChatLogs(string inputLower)
    {
        AppendColoredText("💬 Loading chat logs...\n", PrimaryColor);
        await ShowProgressBar();

        List<ChatMessage> logs;
        string title;

        if (inputLower.Contains("current") || inputLower.Contains("session"))
        {
            logs = ChatLogger.GetCurrentSessionLogs();
            title = "Current Session Chat Log";
        }
        else if (inputLower.Contains("today"))
        {
            logs = ChatLogger.GetLogsByDate(DateTime.Today);
            title = "Today's Chat Log";
        }
        else if (inputLower.Contains("yesterday"))
        {
            logs = ChatLogger.GetLogsByDate(DateTime.Today.AddDays(-1));
            title = "Yesterday's Chat Log";
        }
        else if (inputLower.Contains("password") || inputLower.Contains("phishing") || inputLower.Contains("malware") || 
                 inputLower.Contains("vpn") || inputLower.Contains("encryption") || inputLower.Contains("firewall"))
        {
            // Extract topic from input
            string topic = ExtractTopicFromInput(inputLower);
            logs = ChatLogger.GetLogsByTopic(topic);
            title = $"Chat Log - {topic}";
        }
        else
        {
            logs = ChatLogger.GetAllLogs();
            title = "All Chat Logs";
        }

        string formattedLogs = ChatLogger.FormatChatLog(logs, title);
        AppendColoredText(formattedLogs + "\n\n", TextColor);
        ChatLogger.LogBotMessage($"Displayed {title}: {logs.Count} messages found", "chat");
        
        lblStatus.Text = "Chat logs displayed!";
    }

    private async Task HandleChatSummary()
    {
        AppendColoredText("📊 Generating chat summary...\n", PrimaryColor);
        await ShowProgressBar();

        string summary = ChatLogger.GetChatSummary();
        AppendColoredText(summary + "\n\n", TextColor);
        ChatLogger.LogBotMessage("Chat summary displayed", "chat");
        
        lblStatus.Text = "Chat summary displayed!";
    }

    private async Task HandleChatHelp()
    {
        AppendColoredText("💬 Chat Logging Help\n\n", PrimaryColor, true);
        AppendColoredText("Here are the commands you can use to manage your chat logs:\n\n", TextColor);
        
        AppendColoredText("👁️ Viewing Chat Logs:\n", SecondaryColor);
        AppendColoredText("• 'show chat logs' - View all chat logs\n", TextColor);
        AppendColoredText("• 'show current chat logs' - View current session only\n", TextColor);
        AppendColoredText("• 'show today chat logs' - View today's conversations\n", TextColor);
        AppendColoredText("• 'show yesterday chat logs' - View yesterday's conversations\n", TextColor);
        AppendColoredText("• 'show password chat logs' - View conversations about passwords\n\n", TextColor);
        
        AppendColoredText("📊 Chat Statistics:\n", SecondaryColor);
        AppendColoredText("• 'chat summary' - View conversation statistics\n", TextColor);
        AppendColoredText("• 'conversation summary' - Same as chat summary\n\n", TextColor);
        
        AppendColoredText("🗑️ Managing Chat Logs:\n", SecondaryColor);
        AppendColoredText("• 'clear chat logs' - Clear all chat logs\n", TextColor);
        AppendColoredText("• 'delete chat logs' - Same as clear chat logs\n", TextColor);
        AppendColoredText("• 'export chat logs' - Export logs to a file\n\n", TextColor);
        
        AppendColoredText("💡 Note: All conversations are automatically saved!\n\n", TextColor);
        
        ChatLogger.LogBotMessage("Chat logging help displayed", "chat");
        lblStatus.Text = "Chat help displayed!";
    }

    private async Task HandleClearChatLogs(string inputLower)
    {
        AppendColoredText("🗑️ Clearing chat logs...\n", PrimaryColor);
        await ShowProgressBar();

        if (inputLower.Contains("all") || inputLower.Contains("everything"))
        {
            ChatLogger.ClearAllLogs();
            string successMsg = "All chat logs cleared successfully!";
            AppendMessage("Bot", successMsg, SecondaryColor, TextColor);
            ChatLogger.LogBotMessage(successMsg, "chat");
            lblStatus.Text = "All chat logs cleared!";
        }
        else
        {
            ChatLogger.ClearCurrentSession();
            string successMsg = "Current session chat logs cleared successfully!";
            AppendMessage("Bot", successMsg, SecondaryColor, TextColor);
            ChatLogger.LogBotMessage(successMsg, "chat");
            lblStatus.Text = "Current session cleared!";
        }
    }

    private async Task HandleExportChatLogs()
    {
        AppendColoredText("💾 Exporting chat logs...\n", PrimaryColor);
        await ShowProgressBar();

        try
        {
            string exportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"chat_logs_export_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            ChatLogger.ExportChatLog(exportPath);
            
            string successMsg = "Chat logs exported successfully!";
            AppendMessage("Bot", successMsg, SecondaryColor, TextColor);
            ChatLogger.LogBotMessage(successMsg, "chat");
            
            string fileInfo = $"File saved to: {exportPath}";
            AppendMessage("Bot", fileInfo, TextColor, TextColor);
            ChatLogger.LogBotMessage(fileInfo, "chat");
            lblStatus.Text = "Chat logs exported!";
        }
        catch (Exception ex)
        {
            string errorMsg = $"Error exporting chat logs: {ex.Message}";
            AppendMessage("Bot", errorMsg, ErrorColor, TextColor);
            ChatLogger.LogBotMessage(errorMsg, "chat");
            lblStatus.Text = "Export failed";
        }
    }

    private string ExtractTopicFromInput(string input)
    {
        string inputLower = input.ToLower();
        string[] topics = { "password", "phishing", "malware", "vpn", "encryption", "firewall", "social engineering", "two factor", "data breach", "safe browsing" };
        
        foreach (var topic in topics)
        {
            if (inputLower.Contains(topic))
            {
                return topic;
            }
        }
        
        return "general";
    }

    private string ExtractTaskName(string input)
    {
        // Extract task name from commands like "complete task change password"
        string[] keywords = {
            "complete task", "mark complete", "done",
            "delete task", "remove task"
        };

        foreach (var keyword in keywords)
        {
            if (input.ToLower().Contains(keyword))
            {
                int index = input.ToLower().IndexOf(keyword);
                int start = index + keyword.Length;
                string taskName = input.Substring(start).Trim();
                return taskName;
            }
        }

        return null;
    }

    private async Task ShowPasswordInfo()
    {
        AppendColoredText("🔐 Password Safety Information\n", PrimaryColor, true);
        await ShowProgressBar();
        
        string passwordInfo = "Password safety involves using secure and robust passwords to protect sensitive data and systems from unauthorized access. It includes strategies like creating strong passwords, avoiding weak or repetitive ones, and using tools like password managers to securely store credentials.";
        AppendMessage("Bot", passwordInfo, PrimaryColor, TextColor);
        ChatLogger.LogBotMessage(passwordInfo, "password");
    }

    private async Task ShowPhishingInfo()
    {
        AppendColoredText("🎣 Phishing Awareness Information\n", PrimaryColor, true);
        await ShowProgressBar();
        
        string phishingInfo = "Phishing is a cyberattack where attackers impersonate trusted entities to deceive individuals into revealing sensitive information, such as login credentials or financial details. These attacks often use emails, texts, or fake websites and rely on social engineering techniques to exploit human psychology.";
        AppendMessage("Bot", phishingInfo, PrimaryColor, TextColor);
        ChatLogger.LogBotMessage(phishingInfo, "phishing");
    }

    private async Task ShowMalwareInfo()
    {
        AppendColoredText("🦠 Malware Protection Information\n", PrimaryColor, true);
        await ShowProgressBar();
        
        string malwareInfo = "Malware (malicious software) includes viruses, worms, trojans, ransomware, and spyware designed to damage or gain unauthorized access to systems. To protect yourself: keep software updated, use reputable antivirus programs, don't click suspicious links, and be cautious with downloaded files.";
        AppendMessage("Bot", malwareInfo, PrimaryColor, TextColor);
        ChatLogger.LogBotMessage(malwareInfo, "malware");
    }

    private async Task ShowVpnInfo()
    {
        AppendColoredText("🔒 VPN Security Information\n", PrimaryColor, true);
        await ShowProgressBar();
        
        string vpnInfo = "A Virtual Private Network (VPN) encrypts your internet connection and hides your online activities and location. VPNs are especially important when using public Wi-Fi networks, as they protect your data from potential eavesdroppers on the network.";
        AppendMessage("Bot", vpnInfo, PrimaryColor, TextColor);
        ChatLogger.LogBotMessage(vpnInfo, "vpn");
    }

    private async Task ShowThinkingAnimation()
    {
        if (lblStatus.InvokeRequired)
        {
            lblStatus.Invoke(new Action(async () => await ShowThinkingAnimation()));
            return;
        }

        string[] thinkingDots = { "Thinking", "Thinking.", "Thinking..", "Thinking..." };
        string originalStatus = lblStatus.Text;
        
        for (int i = 0; i < 3; i++)
        {
            lblStatus.Text = thinkingDots[i % thinkingDots.Length];
            await Task.Delay(300);
        }
        
        lblStatus.Text = originalStatus;
    }

    private async Task ShowProgressBar()
    {
        if (progressBar.InvokeRequired)
        {
            progressBar.Invoke(new Action(async () => await ShowProgressBar()));
            return;
        }

        progressBar.Visible = true;
        progressBar.Value = 0;
        
        for (int i = 0; i <= 100; i += 10)
        {
            progressBar.Value = i;
            await Task.Delay(50);
        }
        
        progressBar.Visible = false;
    }

    private void UpdateStatus(string status, Color color = default)
    {
        if (lblStatus.InvokeRequired)
        {
            lblStatus.Invoke(new Action(() => UpdateStatus(status, color)));
            return;
        }

        lblStatus.Text = status;
        if (color != default)
        {
            lblStatus.ForeColor = color;
        }
        else
        {
            lblStatus.ForeColor = MutedTextColor;
        }
    }

    private void ShowSuccessMessage(string message)
    {
        AppendMessage("Bot", message, SuccessColor, TextColor);
        UpdateStatus("Task completed successfully!", SuccessColor);
        
        // Reset status after 3 seconds
        Task.Delay(3000).ContinueWith(_ => 
        {
            UpdateStatus($"Ready to help {_name} with cybersecurity questions and tasks!");
        });
    }

    private void ShowErrorMessage(string message)
    {
        AppendMessage("Bot", message, ErrorColor, TextColor);
        UpdateStatus("An error occurred", ErrorColor);
        
        // Reset status after 3 seconds
        Task.Delay(3000).ContinueWith(_ => 
        {
            UpdateStatus($"Ready to help {_name} with cybersecurity questions and tasks!");
        });
    }

    private void AppendColoredText(string text, Color color, bool bold = false)
    {
        if (rtbOutput.InvokeRequired)
        {
            rtbOutput.Invoke(new Action(() => AppendColoredText(text, color, bold)));
            return;
        }

        rtbOutput.SelectionStart = rtbOutput.TextLength;
        rtbOutput.SelectionLength = 0;

        // Create font with bold if specified
        Font font = bold ? new Font(rtbOutput.Font, FontStyle.Bold) : rtbOutput.Font;
        rtbOutput.SelectionFont = font;
        rtbOutput.SelectionColor = color;
        rtbOutput.AppendText(text);
        rtbOutput.ScrollToCaret();
        
        // Auto-scroll to bottom
        rtbOutput.SelectionStart = rtbOutput.TextLength;
        rtbOutput.ScrollToCaret();
    }

    private void AppendMessage(string sender, string message, Color senderColor, Color messageColor)
    {
        if (rtbOutput.InvokeRequired)
        {
            rtbOutput.Invoke(new Action(() => AppendMessage(sender, message, senderColor, messageColor)));
            return;
        }

        // Add spacing between messages
        if (rtbOutput.TextLength > 0)
        {
            rtbOutput.AppendText("\n");
        }

        // Determine alignment and bubble color
        bool isUser = sender.ToLower() == "you";
        Color bubbleColor = isUser ? PrimaryColor : SurfaceColor;
        Color textColor = isUser ? Color.White : TextColor;
        int bubblePadding = 8;
        int bubbleWidth = rtbOutput.Width - 60;

        // Save current selection
        int start = rtbOutput.TextLength;

        // Add sender name
        rtbOutput.SelectionFont = new Font(rtbOutput.Font, FontStyle.Bold);
        rtbOutput.SelectionColor = senderColor;
        rtbOutput.AppendText($"{sender}: ");

        // Add message bubble
        rtbOutput.SelectionFont = rtbOutput.Font;
        rtbOutput.SelectionColor = textColor;
        rtbOutput.SelectionBackColor = bubbleColor;
        rtbOutput.AppendText(message);
        rtbOutput.SelectionBackColor = rtbOutput.BackColor;

        // Add spacing after message
        rtbOutput.AppendText("\n");

        // Right-align user messages
        if (isUser)
        {
            rtbOutput.Select(start, rtbOutput.TextLength - start);
            rtbOutput.SelectionAlignment = HorizontalAlignment.Right;
            rtbOutput.DeselectAll();
        }
        else
        {
            rtbOutput.Select(start, rtbOutput.TextLength - start);
            rtbOutput.SelectionAlignment = HorizontalAlignment.Left;
            rtbOutput.DeselectAll();
        }

        rtbOutput.ScrollToCaret();
    }

    private bool ContainsKeywords(string input, string keyCategory)
    {
        // Use the improved version from Part1 that handles word boundaries
        return Part1.Part1.ContainsKeywords(input, keyCategory);
    }

    private string ExtractFavouriteTopic(string input)
    {
        string[] keywords = {
            "favourite topic is", "favorite topic is", "favourite topic:", "favorite topic:", 
            "i like", "my favourite", "my favorite"
        };

        foreach (var keyword in keywords)
        {
            int index = input.IndexOf(keyword);
            if (index >= 0)
            {
                int start = index + keyword.Length;
                string topic = input.Substring(start).Trim();
                topic = topic.TrimEnd('.', '!', '?');
                return topic;
            }
        }

        return null;
    }

    private string ExtractTipsTopic(string input)
    {
        foreach (var key in Part1.Part1.KeywordResponses.Keys)
        {
            if (key == "tips" || key == "exit") continue;
            if (ContainsKeywords(input, key))
                return key;
        }
        return null;
    }

    private void TaskCheckTimer_Tick(object sender, EventArgs e)
    {
        // Check for overdue tasks and notify the user
        var overdueTasks = TaskManager.GetOverdueTasks();
        if (overdueTasks.Any())
        {
            // Update status and show notification in the chat
            lblStatus.Text = $"⚠️ You have {overdueTasks.Count} overdue task(s)!";
            AppendColoredText($"⚠️ Reminder: You have {overdueTasks.Count} overdue security task(s):\n", ErrorColor);
            foreach (var task in overdueTasks.Take(3)) // Show first 3 overdue tasks
            {
                AppendColoredText($"• {task.TaskName}: {task.Description}\n", TextColor);
            }
            if (overdueTasks.Count > 3)
            {
                AppendColoredText($"• ... and {overdueTasks.Count - 3} more\n", TextColor);
            }
            AppendColoredText("Type 'show overdue tasks' to see all overdue tasks.\n\n", SecondaryColor);
        }
    }
    //handler for all activities
    private async Task HandleViewActivityLog(string inputLower)
    {
        AppendColoredText("📊 Loading activity log...\n", PrimaryColor);
        await ShowProgressBar();

        List<ActivityLogEntry> activities;
        string title;

        if (inputLower.Contains("all") || inputLower.Contains("everything"))
        {
            activities = ActivityLogger.GetAllActivities();
            title = "All Activities";
        }
        else if (inputLower.Contains("task") || inputLower.Contains("tasks"))
        {
            activities = ActivityLogger.GetActivitiesByCategory("Task");
            title = "Task Activities";
        }
        else if (inputLower.Contains("reminder") || inputLower.Contains("reminders"))
        {
            activities = ActivityLogger.GetActivitiesByCategory("Reminder");
            title = "Reminder Activities";
        }
        else if (inputLower.Contains("quiz") || inputLower.Contains("quizzes"))
        {
            activities = ActivityLogger.GetActivitiesByCategory("Quiz");
            title = "Quiz Activities";
        }
        else if (inputLower.Contains("today"))
        {
            activities = ActivityLogger.GetActivitiesByDate(DateTime.Today);
            title = "Today's Activities";
        }
        else if (inputLower.Contains("recent") || inputLower.Contains("latest"))
        {
            activities = ActivityLogger.GetRecentActivities(5); // Show last 5
            title = "Recent Activities";
        }
        else
        {
            activities = ActivityLogger.GetRecentActivities(10); // Default: last 10
            title = "Recent Activity Log";
        }

        string formattedActivities = ActivityLogger.FormatActivityLog(activities, title);
        AppendColoredText(formattedActivities + "\n\n", TextColor);
        ChatLogger.LogBotMessage($"Displayed {title}: {activities.Count} activities found", "activity");
        
        lblStatus.Text = "Activity log displayed!";
    }

    private async Task HandleActivitySummary()
    {
        AppendColoredText("📊 Generating activity summary...\n", PrimaryColor);
        await ShowProgressBar();

        string summary = ActivityLogger.GetActivitySummary();
        AppendColoredText(summary + "\n\n", TextColor);
        ChatLogger.LogBotMessage("Activity summary displayed", "activity");
        
        lblStatus.Text = "Activity summary displayed!";
    }

//help commands for Activity log
    private async Task HandleActivityHelp()
    {
        AppendColoredText("📊 Activity Logging Help\n\n", PrimaryColor, true);
        AppendColoredText("Here are the commands you can use to view your activity log:\n\n", TextColor);
        
        AppendColoredText("👁️ Viewing Activities:\n", SecondaryColor);
        AppendColoredText("• 'show activity log' - View recent activities (last 10)\n", TextColor);
        AppendColoredText("• 'show recent activities' - View last 5 activities\n", TextColor);
        AppendColoredText("• 'show all activities' - View complete activity history\n", TextColor);
        AppendColoredText("• 'show task activities' - View only task-related activities\n", TextColor);
        AppendColoredText("• 'show reminder activities' - View only reminder activities\n", TextColor);
        AppendColoredText("• 'show quiz activities' - View only quiz activities\n", TextColor);
        AppendColoredText("• 'show today activities' - View today's activities\n\n", TextColor);
        
        AppendColoredText("📊 Activity Statistics:\n", SecondaryColor);
        AppendColoredText("• 'activity summary' - View activity statistics\n", TextColor);
        AppendColoredText("• 'activity stats' - Same as activity summary\n\n", TextColor);
        
        AppendColoredText("💡 Alternative Commands:\n", SecondaryColor);
        AppendColoredText("• 'what have you done for me?' - View recent activities\n", TextColor);
        AppendColoredText("• 'activity history' - View activity log\n\n", TextColor);
        
        AppendColoredText("📋 Tracked Activities:\n", SecondaryColor);
        AppendColoredText("• Task creation, completion, and deletion\n", TextColor);
        AppendColoredText("• Reminder settings and triggers\n", TextColor);
        AppendColoredText("• Quiz starts, completions, and abandonments\n", TextColor);
        AppendColoredText("• Topic discussions and tips requests\n", TextColor);
        AppendColoredText("• User login sessions\n\n", TextColor);
        
        ChatLogger.LogBotMessage("Activity logging help displayed", "activity");
        lblStatus.Text = "Activity help displayed!";
    }

    private async Task HandleStartQuiz(string inputLower)
    {
        AppendColoredText("📋 Starting the Cybersecurity Quiz...\n", PrimaryColor);
        await ShowProgressBar();

        try
        {
            quizQuestions = QuizManager.GetQuiz();
            quizCurrentIndex = 0;
            quizUserAnswers = new List<int>();
            quizScore = 0;
            quizInProgress = true;

            string instructions = QuizManager.GetQuizInstructions();
            AppendColoredText($"📝 {instructions}\n", PrimaryColor);
            ChatLogger.LogBotMessage("Started cybersecurity quiz", "quiz");
            ActivityLogger.LogQuizStarted("cybersecurity", _name);

            await AskNextQuizQuestion();
        }
        catch (Exception ex)
        {
            string errorMsg = $"Error during quiz: {ex.Message}";
            AppendColoredText($"❌ {errorMsg}\n\n", ErrorColor);
            ChatLogger.LogBotMessage(errorMsg, "quiz");
            ActivityLogger.LogQuizAbandoned("cybersecurity", _name);
            lblStatus.Text = "Error during quiz";
            quizInProgress = false;
        }
    }

    private async Task AskNextQuizQuestion()
    {
        if (quizCurrentIndex >= quizQuestions.Count)
        {
            await FinishQuiz();
            return;
        }

        var question = quizQuestions[quizCurrentIndex];
        AppendColoredText($"\nQuestion {quizCurrentIndex + 1}:\n", PrimaryColor);
        AppendColoredText($"{question.Question}\n\n", TextColor);
        for (int i = 0; i < question.Options.Count; i++)
        {
            AppendColoredText($"{i + 1}. {question.Options[i]}\n", TextColor);
        }
        AppendColoredText("\nYour answer: ", AccentColor);
    }

    private async Task FinishQuiz()
    {
        // Calculate and display final score
        int score = QuizManager.CalculateScore(quizUserAnswers, quizQuestions);
        int totalQuestions = quizQuestions.Count;
        string scoreMessage = QuizManager.GetScoreMessage(score, totalQuestions);

        AppendColoredText($"\n📊 Quiz Results:\n", PrimaryColor);
        AppendColoredText($"Score: {score}/{totalQuestions}\n", TextColor);
        AppendColoredText($"{scoreMessage}\n\n", SecondaryColor);

        ChatLogger.LogBotMessage($"Completed cybersecurity quiz with score: {score}/{totalQuestions}", "quiz");
        ActivityLogger.LogQuizCompleted("cybersecurity", score, _name);
        lblStatus.Text = $"Quiz completed! Score: {score}/{totalQuestions}";

        quizInProgress = false;
    }

    private async Task HandleQuizAnswer(string input)
    {
        var question = quizQuestions[quizCurrentIndex];
        int userAnswer = -1;
        if (input.Trim().ToLower() == "quit")
        {
            AppendColoredText("\nQuiz abandoned.\n\n", ErrorColor);
            ChatLogger.LogBotMessage("Quiz abandoned by user", "quiz");
            ActivityLogger.LogQuizAbandoned("cybersecurity", _name);
            quizInProgress = false;
            return;
        }
        if (!int.TryParse(input.Trim(), out userAnswer) || userAnswer < 1 || userAnswer > question.Options.Count)
        {
            AppendColoredText($"Please enter a valid number (1-{question.Options.Count}).\n", ErrorColor);
            return;
        }
        quizUserAnswers.Add(userAnswer - 1);
        if (userAnswer - 1 == question.CorrectAnswer)
        {
            AppendColoredText("✅ Correct! ", SecondaryColor);
            quizScore++;
        }
        else
        {
            AppendColoredText("❌ Incorrect. ", ErrorColor);
        }
        AppendColoredText($"{question.Explanation}\n", TextColor);
        quizCurrentIndex++;
        await AskNextQuizQuestion();
    }

    private async Task<bool> HandleChatCommands(string input, string inputLower)
    {
        // View chat logs
        if (inputLower.Contains("show chat logs") || inputLower.Contains("view chat logs") ||
            inputLower.Contains("chat history") || inputLower.Contains("conversation history") ||
            inputLower.Contains("chat logs") || inputLower.Contains("message history"))
        {
            await HandleViewChatLogs(inputLower);
            return true;
        }

        // Chat summary
        if (inputLower.Contains("chat summary") || inputLower.Contains("conversation summary"))
        {
            await HandleChatSummary();
            return true;
        }

        // Chat help
        if (inputLower.Contains("chat help") || inputLower.Contains("conversation help"))
        {
            await HandleChatHelp();
            return true;
        }

        // Clear chat logs
        if (inputLower.Contains("clear chat logs") || inputLower.Contains("delete chat logs") ||
            inputLower.Contains("clear history") || inputLower.Contains("delete history"))
        {
            await HandleClearChatLogs(inputLower);
            return true;
        }

        // Export chat logs
        if (inputLower.Contains("export chat logs") || inputLower.Contains("save chat logs") ||
            inputLower.Contains("download chat logs"))
        {
            await HandleExportChatLogs();
            return true;
        }

        return false;
    }

    private async Task HandleTimeSpecification(string timeInput)
    {
        if (string.IsNullOrWhiteSpace(_pendingTaskName))
        {
            AppendColoredText("Bot: No pending task to set time for.\n\n", ErrorColor);
            return;
        }

        try
        {
            DateTime reminderTime = TaskManager.ParseTimeInput(timeInput);
            TaskManager.CreateTask(_pendingTaskName, $"Reminder to {_pendingTaskName}", reminderTime);
            
            string successMsg = $"✅ Task created: '{_pendingTaskName}' - Reminder set for {reminderTime:g}";
            AppendColoredText($"Bot: {successMsg}\n\n", SecondaryColor);
            ChatLogger.LogBotMessage(successMsg, "task");
            ActivityLogger.LogTaskCreated(_pendingTaskName, _name);
            
            lblStatus.Text = $"Task created: {_pendingTaskName}";
            _pendingTaskName = null; // Clear pending task
        }
        catch (Exception ex)
        {
            string errorMsg = $"❌ Error creating task: {ex.Message}";
            AppendColoredText($"Bot: {errorMsg}\n\n", ErrorColor);
            ChatLogger.LogBotMessage(errorMsg, "task");
            _pendingTaskName = null; // Clear pending task on error
        }
    }

    private async Task HandleTaskHelp()
    {
        AppendColoredText("📋 Task Management Commands\n\n", PrimaryColor, true);
        AppendColoredText("Here are the exact commands you can type to manage your security tasks:\n\n", TextColor);
        
        AppendColoredText("➕ Creating Tasks:\n", SecondaryColor);
        AppendColoredText("• 'remind me to update my password' - Create a security reminder\n", TextColor);
        AppendColoredText("• 'create task to enable 2FA' - Create a new security task\n", TextColor);
        AppendColoredText("• 'add task to check privacy settings' - Add a new task\n", TextColor);
        AppendColoredText("• 'I need to backup my data' - Natural language task creation\n\n", TextColor);
        
        AppendColoredText("👁️ Viewing Tasks:\n", SecondaryColor);
        AppendColoredText("• 'show tasks' - View all your security tasks\n", TextColor);
        AppendColoredText("• 'view tasks' - Display your task list\n", TextColor);
        AppendColoredText("• 'list tasks' - See your pending tasks\n", TextColor);
        AppendColoredText("• 'see all tasks' - View all tasks\n", TextColor);
        AppendColoredText("• 'all tasks' - Show all tasks\n", TextColor);
        AppendColoredText("• 'view all tasks' - Display all tasks\n\n", TextColor);
        
        AppendColoredText("✅ Completing Tasks:\n", SecondaryColor);
        AppendColoredText("• 'complete task update password' - Mark a task as done\n", TextColor);
        AppendColoredText("• 'mark complete enable 2FA' - Complete a specific task\n", TextColor);
        AppendColoredText("• 'finish task backup data' - Mark task as finished\n\n", TextColor);
        
        AppendColoredText("🗑️ Deleting Tasks:\n", SecondaryColor);
        AppendColoredText("• 'delete task update password' - Remove a task\n", TextColor);
        AppendColoredText("• 'remove task enable 2FA' - Delete a specific task\n", TextColor);
        AppendColoredText("• 'remove all tasks' - Delete all tasks\n", TextColor);
        AppendColoredText("• 'delete all tasks' - Remove all tasks\n", TextColor);
        AppendColoredText("• 'clear all tasks' - Clear all tasks\n\n", TextColor);
        
        AppendColoredText("📊 Task Statistics:\n", SecondaryColor);
        AppendColoredText("• 'task summary' - View task statistics\n", TextColor);
        AppendColoredText("• 'tasks summary' - See task overview\n\n", TextColor);
        
        AppendColoredText("⏰ Time Examples:\n", SecondaryColor);
        AppendColoredText("• 'tomorrow' - Set reminder for tomorrow\n", TextColor);
        AppendColoredText("• '3pm' - Set reminder for 3 PM today\n", TextColor);
        AppendColoredText("• 'in 2 hours' - Set reminder in 2 hours\n", TextColor);
        AppendColoredText("• 'next week' - Set reminder for next week\n", TextColor);
        AppendColoredText("• 'in 7 days' - Set reminder in 7 days\n\n", TextColor);
        
        AppendColoredText("💡 Tips:\n", SecondaryColor);
        AppendColoredText("• You can combine task creation with time: 'remind me to update password tomorrow'\n", TextColor);
        AppendColoredText("• Use natural language: 'I need to check my firewall settings'\n", TextColor);
        AppendColoredText("• Type 'task help' anytime to see these commands again\n\n", TextColor);
        
        ChatLogger.LogBotMessage("Task management help displayed", "task");
        lblStatus.Text = "Task help displayed!";
    }

    private string ExtractTopicFromTipRequest(string input)
    {
        string inputLower = input.ToLower();
        string[] topics = { "password", "phishing", "malware", "vpn", "encryption", "firewall", "social engineering", "two factor", "data breach", "safe browsing" };
        
        foreach (var topic in topics)
        {
            if (inputLower.Contains(topic))
            {
                return topic;
            }
        }
        
        return null;
    }
}