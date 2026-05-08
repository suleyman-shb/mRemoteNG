filepath = 'mRemoteNG/UI/Window/ConnectionTreeWindow.cs'
with open(filepath, 'r') as f:
    lines = f.readlines()

new_lines = []
skip_next_closing = False
for i, line in enumerate(lines):
    if 'foreach (var connection in connectionsToOpen)' in line:
        # Keep the foreach
        new_lines.append(line)
    elif 'Runtime.ConnectionInitiator.OpenConnection(connection);' in line:
        new_lines.append(line)
    elif i > 0 and 'Runtime.ConnectionInitiator.OpenConnection(connection);' in lines[i-1] and '}' in line:
        # Closing brace of foreach
        new_lines.append(line)
    elif i > 1 and 'Runtime.ConnectionInitiator.OpenConnection(connection);' in lines[i-2] and '}' in lines[i-1] and '}' in line:
        # This is the extra closing brace that ends the 'if' block too early or something
        # Let's just rewrite the whole method block to be sure.
        pass
    else:
        new_lines.append(line)

# Actually, it's safer to just replace the whole TvConnections_KeyDown method.
