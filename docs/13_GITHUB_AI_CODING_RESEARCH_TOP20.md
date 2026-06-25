# 13 — GitHub Research: Top 20 Repositories for AI-Assisted Coding / Vibe Coding

Date prepared: 2026-06-25

This list focuses on public GitHub repositories that help coding with AI agents, AI IDE workflows, codebase context, custom instructions, agent skills, or agent orchestration. It is not a ranking by stars only; it is ranked by usefulness for this PRN222 project.

## Best-fit stack for this project

Recommended practical stack:

1. **GitHub Spec Kit** for spec-driven workflow.
2. **This documentation pack** as the project context source.
3. **Codex CLI / Cline / Roo Code / Aider** for coding.
4. **Awesome GitHub Copilot / Cursor Rules / Agent Skills** for reusable instructions.
5. **MCP server lists** only if you need filesystem/GitHub/database tool integrations.

## Top 20 repositories

| Rank | Repository | Category | Why useful for MangaWorkflow | Suggested use |
|---:|---|---|---|---|
| 1 | `openai/codex` | Coding agent CLI | Local coding agent that can edit/run projects; good for .NET implementation tasks. | Use for service/repository/controller implementation. |
| 2 | `Cline/Cline` | IDE coding agent | Reads project structure, edits files, runs commands, and reviews diffs. | Use inside VS Code for multi-file tasks. |
| 3 | `aider-ai/aider` | Terminal pair programmer | Strong git-based workflow; good for small scoped changes. | Use for single feature or bugfix. |
| 4 | `RooCodeInc/Roo-Code` | IDE coding agent | Modes like Code/Architect/Ask/Debug help split work. | Use Architect mode before coding. |
| 5 | `OpenHands/OpenHands` | Software agent platform | Larger autonomous software agent ecosystem. | Use for experimental end-to-end coding, not first choice for student deadline. |
| 6 | `opencode-ai/opencode` | Terminal AI coding TUI | Terminal coding agent with AI assistance for debugging/coding. | Use if you prefer terminal workflow. |
| 7 | `plandex-ai/plandex` | Large-task planning/coding agent | Designed for large tasks spanning many files. | Use for feature planning; keep implementation scoped. |
| 8 | `continuedev/continue` | Open-source coding assistant / CLI checks | Useful for repo rules and automated PR checks. | Add project-specific checks later. |
| 9 | `TabbyML/tabby` | Self-hosted coding assistant | On-prem/open-source Copilot alternative. | Optional if you want local/self-hosted completions. |
| 10 | `microsoft/vscode-copilot-chat` | Copilot Chat extension source | Useful if using VS Code Copilot Chat. | Use with `.github/copilot-instructions.md`. |
| 11 | `github/awesome-copilot` | Copilot instructions/skills/agents | Large community library of Copilot customizations. | Borrow C#/.NET/custom agent instruction patterns. |
| 12 | `VoltAgent/awesome-agent-skills` | Cross-agent skills | Huge curated collection of skills compatible across agents. | Find skills for testing, docs, review, DB work. |
| 13 | `PatrickJS/awesome-cursorrules` | Cursor rules | Project-specific Cursor rules examples. | Adapt rules for PRN222/.NET workflow. |
| 14 | `github/spec-kit` | Spec-driven development | Puts specifications at the center of AI-assisted development. | Use this docs pack as spec source before coding. |
| 15 | `coleam00/context-engineering-intro` | Context engineering template | Teaches how to provide full context to coding agents. | Use structure ideas; do not blindly copy tech stack. |
| 16 | `punkpeye/awesome-mcp-servers` | MCP tools directory | Lists MCP servers for filesystem, GitHub, DB, browser, etc. | Add MCP only after core project is stable. |
| 17 | `modelcontextprotocol/modelcontextprotocol` | MCP specification | Official MCP docs/schema for agent-tool integration. | Useful if connecting AI agent to repo/database tools. |
| 18 | `google-gemini/gemini-cli` | Terminal AI agent | Open-source terminal agent for Gemini. | Alternative coding agent if you use Gemini ecosystem. |
| 19 | `anthropics/claude-code` | Terminal coding agent | Agentic coding tool for terminal/IDE/GitHub workflows. | Useful for planning, coding, explaining code. |
| 20 | `microsoft/ai-agents-for-beginners` | AI agent learning repo | Teaches AI agent concepts. | Learn agent concepts; not directly needed for PRN222 coding. |

## How to use these without chaos

Do not install all tools. Pick one primary coding agent and one rules/skills source.

Recommended combinations:

### Simple student workflow

- GitHub Copilot Chat + this docs pack + `.github/copilot-instructions.md`.

### Strong local coding workflow

- Codex CLI or Aider + this docs pack + AGENTS.md.

### IDE autonomous workflow

- Cline or Roo Code + `.cursor/rules/manga-prn222.mdc`.

### Spec-driven workflow

- GitHub Spec Kit + this SRS + implementation playbook.

## Warning

AI coding tools are powerful but can overbuild. Always constrain prompts with:

- exact module;
- affected files;
- PRN222 evidence;
- database tables;
- non-goals;
- acceptance criteria.
