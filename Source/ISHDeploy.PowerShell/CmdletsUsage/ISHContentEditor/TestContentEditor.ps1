CLS
Import-Module ISHDeploy

$info = @{
  "WebPath" = "F:\InfoShare\";
  "Suffix" = ''
}

# Figure our to if we are using it


#Set-ISHContentEditor -ISHDeployment $info -Domain "localhost" -LicenseKey "4242010ECAAA9C09854BD3D1AA59638234BC3B1D3B044977DBB61A117170C2347C5801F27EDFDDEF46D583E9BDC8CAB17E761D66866D2C82DCD93213908A9815889F545257700354DF4BD7658060B913EB6892FA525BAD0F2C7E16A4ACC95FBB3D6E611E73785A0E72C9873B96C6017C201F2827F89EC32CF24525D85A5FA716EB1B2812FE9AA0720E848788D47178667D99848DA038C39FAB342C2D5281B3132170507A9DEF7E857EB51874163A5880CCBF5BC1DA1975B3588966C96E73208DD074676321FEC88D1BFF0FEB1F4F8971DCAB609325F23807FA9D211389B4160B958BA3A1D2F45FCEF6CEA5DB0A8A930900F5ECC2128D67DD3B5F6883DDB50466E803"
#Set-ISHContentEditor -LicensePath "C:\temp\localhost.txt"

#Pause

Test-ISHContentEditor -Domain "localhost" -ISHDeployment $info 