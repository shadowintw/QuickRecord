using System;
using System.ComponentModel.DataAnnotations;

namespace QuickRecord.Models
{
    /// <summary>
    /// 使用者輸入的快速紀錄
    /// </summary>
    public class QuickNote
    {
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string Content { get; set; } = default!;

        /// <summary>UTC 時間，方便日後跨時區顯示</summary>
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        // ★ 用來標記擁有者 (Azure AD 使用者的 object id)
        public string OwnerId { get; set; }
    }
}
