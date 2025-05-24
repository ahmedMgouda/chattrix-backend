using System.IO;

namespace Chattrix.Core.Models;

/// <summary>
/// Represents a file attachment in a chat message.
/// Data should be a base64 encoded string containing the file bytes.
/// Voice messages are also sent using this attachment model as audio files.
/// </summary>
/// <param name="FileName">Original file name including extension.</param>
/// <param name="Data">Base64 encoded file data.</param>
public record ChatAttachment(string FileName, string Data)
{
    public string Extension => Path.GetExtension(FileName);
}
