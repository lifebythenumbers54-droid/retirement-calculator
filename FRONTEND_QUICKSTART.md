# Frontend Quick Start Guide

## Retirement Calculator - React Frontend

This guide will help you get the frontend up and running quickly.

## Prerequisites

Ensure you have the following installed:
- **Node.js** 18 or higher
- **npm** (comes with Node.js)

Check your versions:
```bash
node --version
npm --version
```

## Installation Steps

### Option 1: Using Setup Script (Recommended for Windows)

1. Open Command Prompt or PowerShell
2. Navigate to the frontend directory:
```bash
cd C:\ClaudeCode\Projects\Tester2\frontend
```

3. Run the setup script:
```bash
setup.bat
```

### Option 2: Manual Installation

1. Navigate to the frontend directory:
```bash
cd C:\ClaudeCode\Projects\Tester2\frontend
```

2. Install dependencies:
```bash
npm install
```

3. Wait for installation to complete (this may take a few minutes)

## Running the Development Server

After installation is complete:

```bash
npm run dev
```

You should see output similar to:
```
VITE v6.0.5  ready in XXX ms

➜  Local:   http://localhost:3000/
➜  Network: use --host to expose
➜  press h + enter to show help
```

## Accessing the Application

Open your browser and navigate to:
```
http://localhost:3000
```

You should see the Retirement Calculator homepage with a placeholder layout.

## Expected Output

When you first load the application, you'll see:
- A header with "Retirement Calculator"
- A subtitle describing the purpose
- A placeholder section showing what input fields will be added in the next phase:
  - Current age
  - Retirement age
  - Retirement account balance (401k/IRA)
  - Taxable account balance
  - Success rate threshold
- A footer with a disclaimer

## API Integration

The frontend is configured to communicate with the backend API at:
```
http://localhost:5000
```

All API requests made to `/api/*` will be automatically proxied to the backend.

### Testing API Connection (Once Backend is Running)

The API client is ready to use:

```javascript
import { checkApiHealth } from './services/apiClient';

// Check if backend is running
const status = await checkApiHealth();
```

## Available Scripts

In the `frontend` directory, you can run:

### `npm run dev`
Starts the development server on port 3000
- Hot module replacement enabled
- Fast refresh for React components
- API proxy to backend enabled

### `npm run build`
Creates an optimized production build in the `dist` folder

### `npm run preview`
Previews the production build locally

## Project Structure

```
frontend/
├── public/              # Static assets
│   └── vite.svg        # Favicon
├── src/
│   ├── components/     # React components (empty, ready for next phase)
│   ├── services/       # API services
│   │   └── apiClient.js
│   ├── App.jsx         # Main component
│   ├── App.css         # App styles
│   ├── main.jsx        # Entry point
│   └── index.css       # Global styles
├── index.html          # HTML template
├── vite.config.js      # Vite configuration
└── package.json        # Dependencies
```

## Next Steps

The frontend is now ready for development. The next phase will include:

1. Creating the input form component
2. Adding form validation
3. Connecting to the backend API
4. Displaying calculation results
5. Adding visualizations with Recharts

## Troubleshooting

### Port 3000 Already in Use
If port 3000 is already in use, you can change it in `vite.config.js`:
```javascript
server: {
  port: 3001,  // Change to any available port
  // ...
}
```

### Installation Fails
1. Clear npm cache: `npm cache clean --force`
2. Delete `node_modules` folder and `package-lock.json`
3. Run `npm install` again

### Development Server Won't Start
1. Check for error messages in the console
2. Ensure all dependencies are installed
3. Verify Node.js version is 18+
4. Try deleting `node_modules` and reinstalling

### Changes Not Showing
1. Ensure the dev server is running
2. Try hard refresh in browser (Ctrl+Shift+R or Cmd+Shift+R)
3. Check for errors in browser console
4. Restart the dev server

## Support

For more detailed information, see:
- `C:\ClaudeCode\Projects\Tester2\frontend\README.md`
- `C:\ClaudeCode\Projects\Tester2\frontend\SETUP_COMPLETE.md`

## Status

Current Phase: **Frontend Initialization - Complete**

Next Phase: **Input Form Components** (to be implemented)
