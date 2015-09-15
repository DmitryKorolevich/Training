Param(
	[string]$RootDeploy
)
Push-Location ".."
echo "Restoring project packages..."
dnu restore --parallel > restore.log
echo "Running project..."
dnx run