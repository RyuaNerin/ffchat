namespace FFChat
{
    partial class frmMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.lsb = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lsb
            // 
            this.lsb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsb.FormattingEnabled = true;
            this.lsb.IntegralHeight = false;
            this.lsb.ItemHeight = 15;
            this.lsb.Location = new System.Drawing.Point(0, 0);
            this.lsb.Name = "lsb";
            this.lsb.ScrollAlwaysVisible = true;
            this.lsb.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lsb.Size = new System.Drawing.Size(420, 237);
            this.lsb.TabIndex = 0;
            this.lsb.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstChat_MouseUp);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 237);
            this.Controls.Add(this.lsb);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FFCHAT";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lsb;






    }
}

