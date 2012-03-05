robocopy  ..\..\..\lib\GSL\ $(OutDir) *.dll /XO /NJH /NP
if errorlevel 4 goto BuildEventFailed
if errorlevel 0 goto end
:BuildEventFailed echo FILECOPY for $(ProjectName) FAILED
exit 1
:end echo FILECOPY for $(ProjectName) COMPLETED OK
exit 0