const http = require('http');
const fs = require('fs');
const path = require('path');

const PORT = process.env.PORT ? Number(process.env.PORT) : 4200;
const ROOT = __dirname;

const MIME = {
  '.html': 'text/html; charset=utf-8',
  '.css': 'text/css; charset=utf-8',
  '.js': 'text/javascript; charset=utf-8',
  '.json': 'application/json; charset=utf-8',
  '.png': 'image/png',
  '.jpg': 'image/jpeg',
  '.jpeg': 'image/jpeg',
  '.gif': 'image/gif',
  '.svg': 'image/svg+xml',
  '.ico': 'image/x-icon',
  '.txt': 'text/plain; charset=utf-8',
};

function safePath(urlPath) {
  const decoded = decodeURIComponent(urlPath.split('?')[0]);
  const normalized = path.normalize(decoded).replace(/^(\.\.[/\\])+/, '');
  return path.join(ROOT, normalized);
}

function send(res, statusCode, headers, body) {
  res.writeHead(statusCode, headers);
  res.end(body);
}

const server = http.createServer((req, res) => {
  if (!req.url) return send(res, 400, { 'Content-Type': 'text/plain; charset=utf-8' }, 'Bad Request');

  // Default to index.html for SPA-like behavior.
  const requested = req.url === '/' ? '/index.html' : req.url;
  const filePath = safePath(requested);

  fs.stat(filePath, (err, stat) => {
    if (!err && stat.isFile()) {
      const ext = path.extname(filePath).toLowerCase();
      const contentType = MIME[ext] || 'application/octet-stream';
      return fs.readFile(filePath, (readErr, data) => {
        if (readErr) return send(res, 500, { 'Content-Type': 'text/plain; charset=utf-8' }, 'Internal Server Error');
        return send(res, 200, { 'Content-Type': contentType, 'Cache-Control': 'no-store' }, data);
      });
    }

    // Fallback to index.html
    const indexPath = path.join(ROOT, 'index.html');
    fs.readFile(indexPath, (readErr, data) => {
      if (readErr) return send(res, 500, { 'Content-Type': 'text/plain; charset=utf-8' }, 'Internal Server Error');
      return send(res, 200, { 'Content-Type': MIME['.html'], 'Cache-Control': 'no-store' }, data);
    });
  });
});

server.listen(PORT, '127.0.0.1', () => {
  console.log(`[frontend] Static server running at http://localhost:${PORT}`);
  console.log(`[frontend] Serving from: ${ROOT}`);
});

