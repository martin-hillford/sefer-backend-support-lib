# Introduction

This library contains shared code for building api's in the Sefer ecosystem.

For configuration purposes it builds upon the  configuration layout as used
in the shared network config configuration and uses the follow configuration options:

| Key                 | Description                                                  |
|---------------------|--------------------------------------------------------------|
| Cors.AllowedOrigins | A list of allowed origins. prefix wildcards are supported.   |
| Cors.MaxAge         | The max duration that the response is cached by the browser. |