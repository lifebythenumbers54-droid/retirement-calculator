#!/bin/bash

echo "================================"
echo "Retirement Calculator Frontend"
echo "Installation Script"
echo "================================"
echo ""

echo "Installing dependencies..."
npm install

if [ $? -ne 0 ]; then
    echo ""
    echo "ERROR: Installation failed!"
    echo "Please check your Node.js and npm installation."
    exit 1
fi

echo ""
echo "================================"
echo "Installation completed successfully!"
echo "================================"
echo ""
echo "To start the development server, run:"
echo "  npm run dev"
echo ""
echo "The app will be available at http://localhost:3000"
echo ""
