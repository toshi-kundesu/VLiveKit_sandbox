# Codex CLI Setup Notes

This is the project-local checklist for using the Codex CLI with this Unity sandbox.

## Install And Start

```powershell
npm install -g @openai/codex
codex
```

Resume previous sessions:

```powershell
codex resume
codex resume --last
codex resume <SESSION_ID>
```

Useful in-session commands:

```text
/init
/new
/model
/compact
/help
/quit
```

## Recommended Local Config

Codex reads the global config from:

```text
%USERPROFILE%\.codex\config.toml
```

Suggested conservative defaults:

```toml
model = "gpt-5-codex"
model_reasoning_effort = "medium"
sandbox_mode = "workspace-write"
approval_policy = "on-request"
file_opener = "vscode"

[tools]
web_search = true

[tui]
notifications = ["agent-turn-complete", "approval-requested"]

[shell_environment_policy]
inherit = "core"
include_only = ["PATH", "HOME", "USER", "USERNAME", "USERPROFILE"]
exclude = ["AWS_*", "AZURE_*", "*TOKEN*", "*SECRET*", "*KEY*"]
```

Set `file_opener = "cursor"` instead if Cursor is the main editor.

## Optional MCP Examples

Playwright MCP:

```powershell
codex mcp add playwright -- npx -y @playwright/mcp
```

Context7 MCP:

```powershell
codex mcp add context7 -- npx -y @upstash/context7-mcp --api-key YOUR_API_KEY
```

Markitdown MCP:

```powershell
codex mcp add markitdown -- uvx markitdown-mcp
```

Check registered MCP servers:

```powershell
codex mcp list
```

## Unity Sandbox Reminders

- Open this repository root in Unity Hub with Unity `2022.3.9f1`.
- Keep generated directories out of commits.
- When changing files inside `Packages/VLiveKit_*`, check whether the package is a submodule and commit the submodule change separately when needed.
- Commit `.meta` files with Unity assets.
