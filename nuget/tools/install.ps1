param($installPath, $toolsPath, $package, $project)

####### initialize variables #######
$contentExtractor = $("{0}\7za.exe" -f $toolsPath)
$archive =  $('{0}\catpic.7z' -f $toolsPath)
$pathUtil = [System.IO.Path]
$projectPath = $('"{0}`"' -f $pathUtil::GetDirectoryName($project.FullName))

####### extract content #######
Push-Location $toolsPath
Invoke-Expression $(".\7za.exe x catpic.7z -o{0} -r" -f $projectPath)
Pop-Location


