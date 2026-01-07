# FileChecker

A microservice that analyzes .NET assemblies for security threats. Used by the Safeturned API to scan uploaded plugin files.

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/health` | GET | Health check with service version |
| `/version` | GET | Returns current service version |
| `/analyze` | POST | Analyzes a .NET assembly and returns threat score + detected features |
| `/validate` | POST | Validates if a file is a valid .NET assembly |

## Usage

**Analyze a file:**
```bash
curl -X POST -F "file=@plugin.dll" http://localhost:5000/analyze
```

**Response:**
```json
{
  "score": 0.75,
  "version": "1.0.0",
  "features": [...],
  "metadata": {
    "company": "...",
    "product": "...",
    "title": "...",
    "copyright": "...",
    "guid": "..."
  }
}
```

## Analyzers

- **BlacklistedCommandAnalyzer** - Detects dangerous command patterns
- **NetworkActivityAnalyzer** - Detects suspicious network activity
