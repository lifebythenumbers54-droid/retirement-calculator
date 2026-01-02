# Retirement Calculator - Frontend

React frontend for the Retirement Calculator application built with Vite.

## Features

- Modern React 18+ with hooks
- Vite for fast development and building
- Axios for API communication
- Recharts for data visualization
- API proxy configuration for seamless backend integration

## Prerequisites

- Node.js 18+
- npm or yarn

## Installation

1. Navigate to the frontend directory:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

## Development

Start the development server:
```bash
npm run dev
```

The application will be available at `http://localhost:3000`

The dev server is configured to proxy API calls to `http://localhost:5000` (the backend API).

## Build

Create a production build:
```bash
npm run build
```

Preview the production build:
```bash
npm run preview
```

## Project Structure

```
frontend/
├── public/              # Static assets
├── src/
│   ├── components/      # React components (to be added in next phase)
│   ├── services/        # API client services
│   │   └── apiClient.js # Axios-based API client
│   ├── App.jsx          # Main application component
│   ├── App.css          # Application styles
│   ├── main.jsx         # Entry point
│   └── index.css        # Global styles
├── index.html           # HTML template
├── vite.config.js       # Vite configuration with API proxy
└── package.json         # Dependencies and scripts
```

## API Configuration

The Vite dev server is configured to proxy all `/api/*` requests to `http://localhost:5000`. This is configured in `vite.config.js`:

```javascript
server: {
  port: 3000,
  proxy: {
    '/api': {
      target: 'http://localhost:5000',
      changeOrigin: true,
      secure: false,
    }
  }
}
```

## Next Steps

The following components will be added in the next phase:
- Input form component
- Results display component
- Visualization component

## Dependencies

### Production
- `react` ^18.3.1 - UI framework
- `react-dom` ^18.3.1 - React DOM rendering
- `axios` ^1.7.9 - HTTP client for API calls
- `recharts` ^2.15.0 - Charting library for visualizations

### Development
- `vite` ^6.0.5 - Build tool and dev server
- `@vitejs/plugin-react` ^4.3.4 - Vite React plugin
