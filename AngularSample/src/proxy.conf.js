const PROXY_CONFIG = [
  {
    context: [
      "/api",
      "/v1"
    ],
    target: "https://localhost:7052",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
