import sys

filepath = 'mRemoteNG/UI/Window/ConnectionTreeWindow.cs'
with open(filepath, 'r') as f:
    content = f.read()

search_text = """<<<<<<< Updated upstream
                    foreach (var node in ConnectionTree.SelectedNodes)
                    {
                        Runtime.ConnectionInitiator.OpenConnection(node);
=======
                    var connectionsToOpen = ConnectionTree.SelectedObjects
                                                         .OfType<ConnectionInfo>()
                                                         .Where(node => node.GetTreeNodeType() == TreeNodeType.Connection)
                                                         .Take(Settings.Default.MaxBulkConnect)
                                                         .ToList();

                    foreach (var connection in connectionsToOpen)
                    {
                        Runtime.ConnectionInitiator.OpenConnection(connection);
>>>>>>> Stashed changes"""

replace_text = """                    var connectionsToOpen = ConnectionTree.SelectedObjects
                                                         .OfType<ConnectionInfo>()
                                                         .Where(node => node.GetTreeNodeType() == TreeNodeType.Connection)
                                                         .Take(Settings.Default.MaxBulkConnect)
                                                         .ToList();

                    foreach (var connection in connectionsToOpen)
                    {
                        Runtime.ConnectionInitiator.OpenConnection(connection);
                    }"""

if search_text in content:
    new_content = content.replace(search_text, replace_text)
    with open(filepath, 'w') as f:
        f.write(new_content)
    print("Conflict resolved.")
else:
    print("Search text not found.")
