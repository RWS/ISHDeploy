﻿$VerbosePreference="Continue"
(Get-WmiObject Win32_Service -filter "name='$name'").Delete()