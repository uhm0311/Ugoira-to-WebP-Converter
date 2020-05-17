var pixiv_login = function(id, pw)
{
	var root = document.getElementById('LoginComponent');
	var idInput;
	var pwInput;
	var loginButton;

	var findLogin = function(element)
	{
		var i = 0;
		
		for (i = 0; i < element.childElementCount; i++)
		{
			var child = element.children[i];
			if (child.attributes.type)
			{
				var childTypeValue = child.attributes.type.value.toLowerCase();
				var childTagName = child.tagName.toLowerCase();
				
				if (childTagName == 'BUTTON'.toLowerCase() && childTypeValue == 'SUBMIT'.toLowerCase())
					loginButton = child;
				else if (childTagName == 'INPUT'.toLowerCase())
				{	
					if (childTypeValue == 'TEXT'.toLowerCase())
						idInput = child;
					else if (childTypeValue == 'PASSWORD'.toLowerCase())
						pwInput = child;
				}
			}
			
			if (child.hasChildNodes())
				findLogin(child);
		}
	};
	findLogin(root);

	idInput.value = id;
	pwInput.value = pw;
	loginButton.click();
};