using System.ComponentModel;

namespace ChatbotForm.Part3;

partial class Form1
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.SuspendLayout();
            
        // Form properties
        this.AutoScaleDimensions = new SizeF(8F, 16F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(900, 700);
        this.BackColor = Color.FromArgb(18, 18, 18);
        this.ForeColor = Color.FromArgb(240, 240, 240);
        this.Text = "🛡️ Cybersecurity Awareness Bot";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(700, 500);
        this.MaximizeBox = true;
        this.MinimizeBox = true;
        this.ShowIcon = true;
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.WindowState = FormWindowState.Normal;
        
        this.ResumeLayout(false);
       
    }

    #endregion
}