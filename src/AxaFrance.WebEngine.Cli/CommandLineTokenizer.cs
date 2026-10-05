namespace AxaFrance.WebEngine.Cli;

internal static class CommandLineTokenizer
{
    public static IReadOnlyList<string> Tokenize(string commandLine)
    {
        var tokens = new List<string>();
        var token = new System.Text.StringBuilder();
        var quoted = false;
        var quoteCharacter = '\0';

        for (var index = 0; index < commandLine.Length; index++)
        {
            var character = commandLine[index];
            if (quoted)
            {
                if (character == quoteCharacter)
                {
                    quoted = false;
                    continue;
                }

                if (character == '\\'
                    && index + 1 < commandLine.Length
                    && commandLine[index + 1] is '"' or '\\')
                {
                    token.Append(commandLine[++index]);
                    continue;
                }

                token.Append(character);
                continue;
            }

            if (character is '"' or '\'')
            {
                quoted = true;
                quoteCharacter = character;
                continue;
            }

            if (char.IsWhiteSpace(character))
            {
                AppendToken(tokens, token);
                continue;
            }

            token.Append(character);
        }

        if (quoted)
            throw new FormatException("The command contains an unterminated quote.");

        AppendToken(tokens, token);
        return tokens;
    }

    private static void AppendToken(List<string> tokens, System.Text.StringBuilder token)
    {
        if (token.Length == 0)
            return;

        tokens.Add(token.ToString());
        token.Clear();
    }
}
