using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoClean.API.Migrations
{
    /// <inheritdoc />
    public partial class FixChatHistoryTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Bảng DB đã tên "ChatHistory" (số ít) — chỉ sync index nếu chưa có.
            // Không rename, không tạo FK mới (đã tồn tại trong DB).
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes
                           WHERE name = 'IX_ChatHistories_UserId_SessionId'
                             AND object_id = OBJECT_ID('ChatHistory'))
                    DROP INDEX [IX_ChatHistories_UserId_SessionId] ON [ChatHistory];

                IF NOT EXISTS (SELECT 1 FROM sys.indexes
                               WHERE name = 'IX_ChatHistory_UserId_SessionId'
                                 AND object_id = OBJECT_ID('ChatHistory'))
                    CREATE INDEX [IX_ChatHistory_UserId_SessionId]
                        ON [ChatHistory] ([UserId], [SessionId]);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
