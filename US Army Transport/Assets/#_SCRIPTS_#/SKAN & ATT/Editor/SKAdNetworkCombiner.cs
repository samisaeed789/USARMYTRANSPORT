using UnityEditor;
#if UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;

public class SKAdNetworkCombiner : MonoBehaviour
{
#if UNITY_IOS
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            UpdateXcodePlist(path);
        }
    }

    private static void UpdateXcodePlist(string path)
    {
        string plistPath = path + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // Add SKAdNetworkItems from the provided XML format

        //pangle
        AddSkAdNetworkItemsFromXml(plist.root, @"
<key>SKAdNetworkItems</key>
  <array>
    <dict>
    <!-- Pangle -->

      <key>SKAdNetworkIdentifier</key>
      <string>22mmun2rn5.skadnetwork</string>
    </dict>
    <dict>
      <key>SKAdNetworkIdentifier</key>
      <string>uw77j35x4d.skadnetwork</string>
    </dict>
    <dict>
      <key>SKAdNetworkIdentifier</key>
      <string>7ug5zh24hu.skadnetwork</string>
    </dict>
    <dict>
      <key>SKAdNetworkIdentifier</key>
      <string>9t245vhmpl.skadnetwork</string>
    </dict>
    <dict>
      <key>SKAdNetworkIdentifier</key>
      <string>kbd757ywx3.skadnetwork</string>
    </dict>
    <dict>
      <key>SKAdNetworkIdentifier</key>
      <string>a8cz6cu7e5.skadnetwork</string>
    </dict>
    <dict>
      <key>SKAdNetworkIdentifier</key>
      <string>578prtvx9j.skadnetwork</string>
    </dict>
    <dict>
      <key>SKAdNetworkIdentifier</key>
      <string>5tjdwbrq8w.skadnetwork</string>
    </dict>
    <dict>
      <key>SKAdNetworkIdentifier</key>
      <string>hs6bdukanm.skadnetwork</string>
    </dict>
    <dict>
      <key>SKAdNetworkIdentifier</key>
      <string>k674qkevps.skadnetwork</string>
    </dict>
    <dict>
      <key>SKAdNetworkIdentifier</key>
      <string>dbu4b84rxf.skadnetwork</string>
    </dict>

    <!-- Meta -->

     <dict>
        <key>SKAdNetworkIdentifier</key>
        <string>v9wttpbfk9.skadnetwork</string>
    </dict>
    <dict>
        <key>SKAdNetworkIdentifier</key>
        <string>n38lu8286q.skadnetwork</string>
    </dict>

  </array>

        ");
        // Save the modified plist
        plist.WriteToFile(plistPath);
    }

    private static void AddSkAdNetworkItemsFromXml(PlistElementDict rootDict, string xmlContent)
    {
        // Parse XML content and add SKAdNetworkItems to the plist
        PlistElementArray skAdNetworkItemsArray = rootDict.CreateArray("SKAdNetworkItems");

        // Define the start and end tags for parsing
        const string startTag = "<string>";
        const string endTag = "</string>";
        const string skAdNetworkIdKey = "<key>SKAdNetworkIdentifier</key>";

        // Parse each <dict> element from the XML
        string[] dictElements = xmlContent.Split(new[] { "<dict>" }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string dictElement in dictElements)
        {
            if (!dictElement.Contains(skAdNetworkIdKey)) continue;

            // Extract SKAdNetworkIdentifier value from each <dict>
            int start = dictElement.IndexOf(startTag) + startTag.Length;
            int end = dictElement.IndexOf(endTag);
            string skAdNetworkIdentifier = dictElement.Substring(start, end - start);

            // Add SKAdNetworkIdentifier to the plist
            PlistElementDict dict = skAdNetworkItemsArray.AddDict();
            dict.SetString("SKAdNetworkIdentifier", skAdNetworkIdentifier);
        }
    }
#endif
}
