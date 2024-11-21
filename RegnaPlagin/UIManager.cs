using System;
using System.Collections.Generic;
using Renga;

namespace RegnaPlagin
{
    public class UIManager : IPlugin
    {
        const string _menuName = "! Тест";
        const string _menuActionItemName = "Показать координаты ограничивающего объема";
        private readonly Guid _menuGuid = new Guid("{FEE98707-893E-4378-B398-CC8B564A7FE3}");
        private readonly List<ActionEventSource> _actionEventsCollection = new List<ActionEventSource>();

        public bool Initialize(string pluginFolder)
        {
            Application app = new Application();
            IUI uiApp = app.UI;

            IContextMenu contextMenu = uiApp.CreateContextMenu();
            IContextMenuNodeItem menuNode = contextMenu.AddNodeItem();
            menuNode.DisplayName = _menuName;

            IAction contextMenuAction = uiApp.CreateAction();
            contextMenuAction.DisplayName = _menuActionItemName;
            menuNode.AddActionItem(contextMenuAction);

            ActionEventSource eventSource = new ActionEventSource(contextMenuAction);
            BoundingBox HandlerInstance = new BoundingBox(app, uiApp);
            eventSource.Triggered += HandlerInstance.ActinHandler;
            _actionEventsCollection.Add(eventSource);
            
            uiApp.AddContextMenu(_menuGuid, contextMenu, ViewType.ViewType_View3D, ContextMenuShowCase.ContextMenuShowCase_Scene);
            return true;
        }

        public void Stop()
        {
            foreach (var eventSource in _actionEventsCollection)
                eventSource.Dispose();

            _actionEventsCollection.Clear();
        }
    }
}
