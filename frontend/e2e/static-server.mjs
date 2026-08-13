import { createReadStream, existsSync, statSync } from 'node:fs';
import { createServer } from 'node:http';
import { extname, join, normalize } from 'node:path';

const root = join(process.cwd(), 'dist', 'frontend', 'browser');
const contentTypes = {
  '.css': 'text/css; charset=utf-8',
  '.html': 'text/html; charset=utf-8',
  '.ico': 'image/x-icon',
  '.js': 'text/javascript; charset=utf-8',
  '.json': 'application/json; charset=utf-8',
  '.png': 'image/png',
  '.svg': 'image/svg+xml',
  '.woff2': 'font/woff2',
};

const server = createServer((request, response) => {
  const requestPath = decodeURIComponent((request.url ?? '/').split('?')[0]);
  const relativePath = normalize(requestPath).replace(/^[/\\]+/, '');
  let file = join(root, relativePath || 'index.html');
  if (!file.startsWith(root) || !existsSync(file) || statSync(file).isDirectory()) {
    file = join(root, 'index.html');
  }
  response.setHeader('Content-Type', contentTypes[extname(file)] ?? 'application/octet-stream');
  createReadStream(file).pipe(response);
}).listen(4200, '127.0.0.1');

// Playwright terminates this process after the suite. The fallback prevents Windows CI shells from
// retaining the server if process-tree termination is unavailable.
setTimeout(() => server.close(), 30_000);
