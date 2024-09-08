const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7270';

const PROXY_CONFIG = [
  {
    context: [
      "/api",  // Proxy all API requests
    ],
    target: target,
    secure: false,  // Disable SSL validation in development
    changeOrigin: true,
    logLevel: "debug"  // Enable detailed logging
  }
];

module.exports = PROXY_CONFIG;
