function logger{
    Param( [Parameter(Position=0,Mandatory=$true)][string] $test_id,
    [Parameter(Position=1,Mandatory=$true)][string] $test_name,
    [Parameter(Position=2,Mandatory=$true)][string] $test_result,
    [Parameter(Position=3,Mandatory=$true)][string] $test_exception
    )

    $logObject = New-Object -TypeName PSObject
    $logObject | Add-Member -Name 'test_id' -MemberType Noteproperty -Value $test_id
    $logObject | Add-Member -Name 'test_name' -MemberType Noteproperty -Value $test_name
    $logObject | Add-Member -Name 'test_result' -MemberType Noteproperty -Value $test_result
    $logObject | Add-Member -Name 'test_exception' -MemberType Noteproperty -Value $test_exception
    $global:logArray += $logObject

    
     
}

function Set-Style(){
$a = "<style>"

$a = $a+   "  body {font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;}"

$a = $a+     "table{ border-collapse: collapse; border: none; font: 10pt Verdana, Geneva, Arial, Helvetica, sans-serif; color: black; margin-bottom: 10px;}"
 
$a = $a+    " table td{font-size: 16px; padding-left: 0px; padding-right: 20px; text-align: left;}"
 
$a = $a+     "table th {  font-size: 20px;  font-weight: bold; padding-left: 0px;padding-right: 20px; text-align: left;background: green; }"
 
$a = $a+     "h2{ clear: both; font-size: 130%;color:#354B5E; }"
 
$a = $a+     "h3{ clear: both;font-size: 75%; margin-left: 20px;  margin-top: 30px;  color:#475F77; }"
 
$a = $a+     "p{ margin-left: 20px; font-size: 12px; }"
$a = $a+     "table.list{ float: left; }"
 
 $a = $a+    "table.list td:nth-child(1){font-weight: bold;border-right: 1px grey solid;text-align: center;}"
 
 $a = $a+    "table.list td:nth-child(2){ padding-left: 7px; } table tr:nth-child(even) td:nth-child(even){ background: #BBBBBB; }"
 $a = $a+   " table tr:nth-child(odd) td:nth-child(odd){ background: #F2F2F2; }"
 $a = $a+    "table tr:nth-child(even) td:nth-child(odd){ background: #DDDDDD; }"
 $a = $a+    "table tr:nth-child(odd) td:nth-child(even){ background: #E5E5E5; }"
 $a = $a+    "div.column { width: 320px; float: left; }"
  $a = $a+  " div.first{ padding-right: 20px; border-right: 1px grey solid; }"
  $a = $a+   "div.second{ margin-left: 30px; }"
  $a = $a+   "table{ margin-left: 20px; }"

  $a = $a+   "</style>"

  return $a
}

function Edit-LogHtml([string] $targetHTML){


(Get-Content $targetHTML) | 
Foreach-Object {$_ -replace '<td>Failed</td>',"<td style='color: red'>Failed</td>"}  | 
Out-File $targetHTML

(Get-Content $targetHTML) | 
Foreach-Object {$_ -replace '<td>Passed</td>',"<td style='color: green'>Passed</td>"}  | 
Out-File $targetHTML

(Get-Content $targetHTML) | 
Foreach-Object {$_ -replace '<td>Blocked</td>',"<td style='color: yellow'>Blocked</td>"}  | 
Out-File $targetHTML
}

function Assert_IsTrue($boolValue, $comandName, $testId) {
    if ($boolValue) {
        Write-Host $comandName -NoNewline
        Write-Host " Passed" -foregroundcolor "green" 
        logger $testId $comandName "Passed" " "
    }

    else {
            Write-Host $comandName -NoNewline 
            Write-Host " Failed"  -foregroundcolor "red"
            logger $testId $comandName "Failed" " "
    }
 }
 
function TestIsBlocked($comandName, $testId, $message) {
    Write-Host $comandName -NoNewline
    Write-Host  " blocked"  -foregroundcolor "yellow"
    logger $testId $comandName "Blocked" $message
 }