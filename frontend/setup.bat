@echo off
echo ================================
echo Retirement Calculator Frontend
echo Installation Script
echo ================================
echo.

echo Installing dependencies...
call npm install

if %errorlevel% neq 0 (
    echo.
    echo ERROR: Installation failed!
    echo Please check your Node.js and npm installation.
    pause
    exit /b %errorlevel%
)

echo.
echo ================================
echo Installation completed successfully!
echo ================================
echo.
echo To start the development server, run:
echo   npm run dev
echo.
echo The app will be available at http://localhost:3000
echo.
pause
