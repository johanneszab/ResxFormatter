# Resx Formatter
Resx Formatter is an extension that allows to define the sort order of the XML data nodes when saving a resx file.
Therefore, this extension can help you/your team to mitigate merge conflicts by sorting the resx keys in a particular,
well-defined order.

# Installation
You can use the [latest resxformatter.rider.x.y.z.zip](https://github.com/johanneszab/ResxFormatter/releases) from the 
release page and install via `Settings -> Plugins -> Plugin Settings Wheel -> Install Plugin from Disk...` as shown below:

![Plugin installation via file](https://raw.githubusercontent.com/johanneszab/ResxFormatter/refs/heads/master/media/installation_file.png "Sorted when saved")

# Usage
There are currently two modes available:

1. format resx file when saved
2. format resx file via context action

## Format when saved
Per default, ResxFormatter sorts the XML data nodes of the resource file by StringComparison.OrdinalIgnoreCase. However,
that can be changed in the settings. With this settings enabled, ResxFormatter listens for saves of .resx files in
Riders virtual file system and intercepts before the save happens to sort the file. There is no user action necessary
and the file will always be correctly sorted, similar to ResXManager for visual studio. The gif below shows this it in
action.

![ResxFormatter sorts on save](https://raw.githubusercontent.com/johanneszab/ResxFormatter/refs/heads/master/media/resxformatter.gif "Sorted when saved")

## Format via context action
Alternatively, Resx Formatter can also be invoked via context action from the gutter within .resx files

![ResxFormatter Context Action](https://raw.githubusercontent.com/johanneszab/ResxFormatter/refs/heads/master/media/context_action.png "Context action")

# Settings

You can disable the sort on save functionality via the settings, and share a common
[Settings.ResxFormatter](https://github.com/johanneszab/ResxFormatter/blob/master/src/ResxFormatter/Options/DefaultSettings.json)
within your vcs to share common settings between developers.

![ResxFormatter Settings](https://raw.githubusercontent.com/johanneszab/ResxFormatter/refs/heads/master/media/settings.png "Settings")

# How to build

To build the plugin, you need to run `gradle :buildPlugin` from `src\ResxFormatter.Extension.Rider\`. Afterwards, in
`src\ResxFormatter.Extension.Rider\build\distributions` you'll find a `resxformatter.rider-x.y.z.zip` which can be
installed via Rider as plugin from disk.  
Alternatively, you can open folder `src\ResxFormatter.Extension.Rider\` as gradle project in IntelliJ. 

# How to develop

For the R# (backend), you can open `src\ResxFormatter.Rider.sln` in Rider.  
For the IntelliJ part (frontend), you can open `src\ResxFormatter.Extension.Rider\` as gradle project in IntelliJ.

In rider, you can run the runConfig `Rider (Windows/Unix)`. This will generate the rd protocol, install the plugin in
a sandboxed rider and immediately start it. You can then attach with the debugger to the newly spawned Rider.Backend
process for debugging.
