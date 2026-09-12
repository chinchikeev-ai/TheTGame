# Namespace Policy

Current policy: keep runtime classes in the global namespace.

The runtime assembly declares `rootNamespace: TheTroyGame`, but the existing codebase is not yet namespaced. A partial migration would create inconsistent references and extra risk for AI agents.

Rules:
- Do not introduce `namespace TheTroyGame` in isolated files.
- Do not migrate namespaces as part of unrelated work.
- New runtime classes stay in the global namespace for now.
- Architecture validation treats partial namespace migration as a violation.

A future namespace migration must be a dedicated repository-wide task, preserve Unity `.meta` GUIDs, update all references coherently, and pass EditMode, PlayMode and build validation before merge.
