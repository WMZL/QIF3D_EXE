using System.Collections;
using System.Collections.Generic;

public class BllTreeNodeInfo
{
    private List<BllTreeNodeInfo> children = new List<BllTreeNodeInfo>();
    public List<BllTreeNodeInfo> Children { get { return children; } }

    public BllTreeNodeInfo Parent { get; set; }

    public List<DevNodeInfo> devNodes = new List<DevNodeInfo>();
    public List<DevNodeInfo> DevNodes { get { return devNodes; } }

    public string TreeID { get; set; }
    public string TreeParentID { get; set; }

    public string NodeName { get; set; }
    public string Code { get; set; }

    public int BindType { get; set; }

    public string BindID { get; set; }

    public int Sort { get; set; }
}

public class DevNodeInfo
{
    public string DevID { get; set; }

    public string NodeID { get; set; }

    public string Name { get; set; }

    public int NodeType { get; set; }
}
