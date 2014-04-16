// http://www.tinymce.com/wiki.php/Tutorials:Creating_a_plugin
tinymce.PluginManager.add('tugberk', function (editor, url) {

    // Add a button that opens a window
    editor.addButton('tugberk-btn', {
        text: 'My button',
        icon: true,
        onclick: function () {
            // Open window
            editor.windowManager.open({
                title: 'Example pluginn',
                body: [
                    { type: 'textbox', name: 'title', label: 'Title' }
                ],
                onsubmit: function (e) {
                    // Insert content when the window form is submitted
                    editor.insertContent('Title: ' + e.data.title);
                }
            });
        }
    });

    // Adds a menu item to the tools menu
    editor.addMenuItem('example', {
        text: 'Insert Image',
        context: 'tools',
        onclick: function () {

            // Open window with a specific url
            editor.windowManager.open({
                title: 'TinyMCE site',
                url: '/home/image',
                width: 200,
                height: 200,
                buttons: [
                    {
                        text: 'Close',
                        onclick: 'close'
                    },
                    {
                        text: 'Insert',
                        onclick: function (e) {
                            console.log(e);
                            console.log(editor.windowManager);
                            console.log(editor.windowManager.getParams());
                        }
                    }
                ]
            },
            {
                arg1: 42,
                arg2: "Hello world"
            });
        }
    });

});