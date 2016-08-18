a modification has been made to paste pluging in order to remove the class attributes of incoming html element

			html = html.replace(/class=\"[^\"]+\"/g, '');
