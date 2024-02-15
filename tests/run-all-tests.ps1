param(
    [string]$testsFilePath,
    [string]$templateRootPath = ''
)

$config = Get-Content $testsFilePath | ConvertFrom-Json

if ($templateRootPath -eq '') {
    $templateRootPath = $config.templateRootPath
}

# Install template
dotnet new install "$($templateRootPath)/$($config.templateFolder)" --force

foreach ($_ in $config.tests) {
    # Generate template
    Start-Job -Name $_.name -WorkingDirectory "$($templateRootPath)" -ScriptBlock {Invoke-Expression "dotnet new $($args[0]) --force -o $($args[1]) $($args[2])"} -ArgumentList $config.templateName,"$($config.templateArtifact)/$($_.name)",$_.values
    Receive-Job -Name $_.name -Wait

    # Run tests
    dotnet test ./VerifyTests --configuration release -e TemplateProjectDirectory="$($templateRootPath)" -e TemplateOutputDirectory="$($config.templateArtifact)/$($_.name)" -e TestOutputDirectory="$($config.testOutputPath)/$($_.name)"
}