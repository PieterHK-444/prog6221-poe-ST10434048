# 🛡️ Cybersecurity Awareness Chatbot

A comprehensive Windows Forms application designed to educate users about cybersecurity best practices through an interactive chatbot interface. Built with C# and .NET 8.0, this application combines educational content with practical task management features.

## 🌟 Features

### 🎯 Core Functionality
- **Interactive Chat Interface**: Modern, dark-themed UI with real-time conversation capabilities
- **Cybersecurity Education**: Comprehensive coverage of 10 key cybersecurity topics
- **Task Management**: Create, track, and manage security-related tasks with reminders
- **Quiz System**: Interactive cybersecurity quiz with multiple-choice and true/false questions
- **Activity Logging**: Track user interactions and learning progress
- **Chat Logging**: Persistent conversation history with export capabilities

### 📚 Educational Topics Covered
1. **Password Security** - Best practices for creating and managing strong passwords
2. **Phishing Awareness** - Identifying and avoiding email scams and fake communications
3. **Safe Browsing** - Secure web navigation and HTTPS understanding
4. **Malware Protection** - Understanding viruses, spyware, and ransomware
5. **Social Engineering** - Recognizing psychological manipulation tactics
6. **Two-Factor Authentication (2FA)** - Multi-factor authentication benefits
7. **VPN Usage** - Virtual Private Networks and secure connections
8. **Data Breach Response** - What to do when data is compromised
9. **Firewall Protection** - Network security and traffic filtering
10. **Encryption** - Data protection and cryptography basics

### 🎮 Interactive Features
- **Natural Language Processing**: Understands conversational input and context
- **Sentiment Analysis**: Detects user mood and provides empathetic responses
- **User Memory**: Remembers user preferences and favorite topics
- **Audio Feedback**: Welcome sound effects for enhanced user experience
- **Visual Feedback**: Progress bars, typing animations, and color-coded responses

### 📋 Task Management System
- **Smart Task Creation**: Natural language task creation (e.g., "remind me to change password in 7 days")
- **Time-based Reminders**: Set tasks with specific timeframes (hours, days, weeks)
- **Overdue Notifications**: Automatic alerts for pending security tasks
- **Task Completion Tracking**: Mark tasks as completed with progress tracking
- **Task Summary Reports**: Overview of all security tasks and their status

### 🧠 Quiz System
- **10 Interactive Questions**: Mix of multiple-choice and true/false questions
- **Real-time Scoring**: Immediate feedback on quiz performance
- **Educational Explanations**: Detailed explanations for each answer
- **Performance Assessment**: Score-based feedback and encouragement

## 🚀 Getting Started

### Prerequisites
- **Windows 10/11** (Windows Forms application)
- **.NET 8.0 Runtime** or later
- **Visual Studio 2022** (for development) or **Visual Studio Code**

### Installation

1. **Clone the repository**
   ```bash
   git clone [repository-url]
   cd prog6221-poe-ST10434048
   ```

2. **Open the solution**
   - Open `ChatbotForm.sln` in Visual Studio 2022
   - Or use Visual Studio Code with C# extension

3. **Build and Run**
   - Press `F5` to build and run the application
   - Or use `Ctrl+F5` to run without debugging

### Running the Application
1. Launch the application
2. Enter your name when prompted
3. Start chatting with the bot about cybersecurity topics
4. Use commands like "help" to see available features

## 💬 Usage Guide

### Basic Commands
- **"help"** - Show available commands and features
- **"quiz"** - Start the cybersecurity quiz
- **"topics"** - List available cybersecurity topics
- **"tasks"** - View your security tasks
- **"logs"** - View chat history

### Task Management
- **"remind me to [task] in [time]"** - Create a new security task
- **"complete [task]"** - Mark a task as completed
- **"delete [task]"** - Remove a task
- **"task summary"** - View all tasks and their status

### Educational Queries
- **"tell me about [topic]"** - Learn about specific cybersecurity topics
- **"tips for [topic]"** - Get practical tips for any topic
- **"my favorite topic is [topic]"** - Set your preferred learning area

### Chat Management
- **"view chat logs"** - See conversation history
- **"export logs"** - Export chat history to file
- **"clear logs"** - Clear chat history
- **"chat summary"** - Get overview of your learning progress

## 🏗️ Project Structure

```
ChatbotForm/
├── ChatbotForm/
│   ├── Core/
│   │   ├── Models/
│   │   │   └── SecurityTask.cs          # Task data model
│   │   └── Services/
│   │       ├── ActivityLogger.cs        # User activity tracking
│   │       ├── ChatLogger.cs            # Conversation logging
│   │       ├── NLPProcessor.cs          # Natural language processing
│   │       ├── QuizManager.cs           # Quiz system management
│   │       └── TaskManager.cs           # Task management system
│   ├── Part1/
│   │   └── Part1.cs                     # Console-based chatbot (original)
│   ├── Part2/
│   │   ├── part2.cs                     # Enhanced features
│   │   ├── SecurityTasks.cs             # Task-related functionality
│   │   ├── SentimentCheck.cs            # Sentiment analysis
│   │   ├── TaskManager.cs               # Task management logic
│   │   └── UserMemory.cs                # User preference storage
│   ├── Part3/
│   │   ├── Form1.cs                     # Main Windows Forms UI
│   │   └── Form1.Designer.cs            # UI designer file
│   ├── pictures/
│   │   └── chat-icon.ico                # Application icon
│   ├── Sounds/
│   │   └── Welcome.wav                  # Welcome audio
│   └── Program.cs                       # Application entry point
└── ChatbotForm.sln                      # Solution file
```

## 🔧 Technical Details

### Architecture
- **Windows Forms Application** - Modern UI with dark theme
- **MVC Pattern** - Separation of concerns with services and models
- **Event-Driven Programming** - Responsive user interface
- **Data Persistence** - JSON-based storage for tasks and logs

### Key Technologies
- **.NET 8.0** - Latest .NET framework
- **C#** - Primary programming language
- **Windows Forms** - User interface framework
- **JSON** - Data serialization and storage
- **System.Media** - Audio playback capabilities

### Data Storage
- **security_tasks.json** - Task data persistence
- **chat_logs.json** - Conversation history
- **activity_logs.json** - User activity tracking

## 🎨 UI Features

### Modern Design
- **Dark Theme** - Easy on the eyes with modern color scheme
- **Gradient Effects** - Visual appeal with subtle gradients
- **Responsive Layout** - Adapts to different window sizes
- **Color-Coded Messages** - Different colors for different message types

### User Experience
- **Typing Animations** - Visual feedback during processing
- **Progress Indicators** - Show task completion progress
- **Status Updates** - Real-time status information
- **Keyboard Shortcuts** - Enter key for sending messages

## 📊 Learning Analytics

The application tracks various metrics to help users understand their learning progress:

- **Topic Engagement** - Which cybersecurity topics you've explored
- **Quiz Performance** - Your scores and improvement over time
- **Task Completion** - How well you're following through on security tasks
- **Session Duration** - Time spent learning about cybersecurity

## 🔒 Security Features

- **Local Data Storage** - All data stored locally on your machine
- **No External Dependencies** - No internet connection required
- **Privacy-First** - No data collection or sharing
- **Secure File Handling** - Safe JSON file operations

## 🐛 Troubleshooting

### Common Issues

1. **Application won't start**
   - Ensure .NET 8.0 Runtime is installed
   - Check Windows compatibility

2. **Audio not playing**
   - Verify system audio is enabled
   - Check if Welcome.wav file exists in Sounds folder

3. **Tasks not saving**
   - Ensure write permissions in application directory
   - Check if security_tasks.json is not locked by another process

4. **UI not displaying correctly**
   - Try running as administrator
   - Check Windows display scaling settings

## 🤝 Contributing

This is an educational project. If you'd like to contribute:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📝 License

This project is created for educational purposes as part of the POE (Portfolio of Evidence) assignment.

## 👨‍💻 Author

**Student ID**: ST10434048  
**Course**: Programming 6221  
**Institution**: Varsity College Sandton

## 📚 Learning Objectives

This project demonstrates:
- **Object-Oriented Programming** - Classes, inheritance, polymorphism
- **Event Handling** - User interface responsiveness
- **Data Persistence** - File I/O and JSON serialization
- **User Experience Design** - Intuitive interface design
- **Educational Technology** - Interactive learning systems
- **Cybersecurity Awareness** - Practical security education

---

**Stay safe online! 🛡️**  
*This chatbot is your friendly guide to cybersecurity best practices.* 