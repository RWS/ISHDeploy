InfoShare.Deployment Layers overview
================================================

## Overview
CommandLet functionality is implemented using top to bottom approach. When at first we are accessing the very top level of which is more generic and abstract. And then descending layer by layer making functionality less generic and more specific.

## CommandLet layer
At command let layer functionality which is specific to the CommandLet itself defined, it`s its Name, calling parameters and associated Operation name.
```cs
[Cmdlet(VerbsLifecycle.Enable, "ISHExternalPreview", SupportsShouldProcess = false)]
```

## Operation layer
At operation layer defined set of necessary actions which has to be executed to achieve results expected by CommandLet execution.
```cs
public DisableISHUIContentEditorOperation(ILogger logger, ISHPaths paths)
{
    _invoker = new ActionInvoker(logger, "Disabling InfoShare Content Editor");
            
    _invoker.AddAction(new XmlBlockCommentAction(logger, paths.FolderButtonbar, new [] { CommentPatterns.XopusAddCheckOut, CommentPatterns.XopusAddUndoCheckOut }));
    _invoker.AddAction(new XmlBlockCommentAction(logger, paths.InboxButtonBar, CommentPatterns.XopusAddCheckOut));
    _invoker.AddAction(new XmlBlockUncommentAction(logger, paths.InboxButtonBar, new [] { CommentPatterns.XopusRemoveCheckoutDownload, CommentPatterns.XopusRemoveCheckIn }));
    _invoker.AddAction(new XmlBlockCommentAction(logger, paths.LanguageDocumentButtonBar, CommentPatterns.XopusAddCheckOut));
    _invoker.AddAction(new XmlBlockUncommentAction(logger, paths.LanguageDocumentButtonBar, new[] { CommentPatterns.XopusRemoveCheckoutDownload, CommentPatterns.XopusRemoveCheckIn }));
}
```


## Action layer
Action is the basic, isolated and granular functionality which is shared between operations.
```cs
public class FileCopyAction : BaseAction, IRestorableAction
{
    /// <summary>
    /// Executes the action
    /// </summary>
    public override void Execute()
    {
        _fileManager.CopyToDirectory(_sourcePath, _destinationPath, _force);
    }
}
```

