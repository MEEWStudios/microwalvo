const insertElement = document.createElement('div');

function request(url, method, headers, body) {
	method = method ? method : 'GET';

	return fetch(url, {
		method: method,
		cache: 'no-cache',
		headers: headers ? headers : undefined,
		body: method == 'GET' || method == 'HEAD' || !body ? undefined : body
	});
}

function createHTML(htmlString, parentElement) {
	let returnValue = null;

	insertElement.insertAdjacentHTML('beforeend', htmlString);

	if (insertElement.childNodes.length == 0) {
		return null;
	} else {
		returnValue = [...insertElement.childNodes];
	}

	// Remove new nodes from dummy element
	while (insertElement.lastChild) {
		insertElement.removeChild(insertElement.lastChild);
	}

	// Insert elements into new parent
	if (parentElement) {
		for (let element of returnValue) {
			parentElement.appendChild(element);
		}
	}

	if (returnValue.length == 1) {
		return returnValue[0];
	}

	return returnValue;
};

function delay(time) {
	return new Promise(resolve => {
		setTimeout(resolve, time);
	});
}

function isInHeirarchyOf(element, parent) {
    while (element) {
        if (element === parent) {
            return true;
        } else {
            element = element.parentElement;
        }
    }
    
    return false;
}

const getScrollbarWidth = (() => {
    let width = undefined;

    return () => {
        if (width !== undefined) {
            // Cache the value once it's been calculated
            return width;
        } else {
            let container = document.createElement('div');
            container.style.visibility = 'hidden';
            container.style.overflow = 'scroll';
            let child = document.createElement('div');

            container.appendChild(child);
            document.body.appendChild(container);

            width = container.offsetWidth - child.offsetWidth;
            document.body.removeChild(container);

            return width;
        }
    }
})();

document.addEventListener('DOMContentLoaded', async () => {
	let headerLinks = document.querySelector('header').getElementsByTagName('a');
	let footerLinks = document.querySelector('footer').getElementsByTagName('a');
	
	for (let link of headerLinks) {
		let isTargetPage = link.getAttribute('href') == location.pathname;
		let isSubPage = link.dataset.loose !== undefined && location.pathname.substring(0, link.getAttribute('href').length) == link.getAttribute('href');
		
		// If the current page is either the target or a subpage of the target
		if (isTargetPage || isSubPage) {
			// Mark as active
			link.classList.add('active');
		}
		
		// If the current page is the target page
		if (isTargetPage) {
			// Prevent reloading the page
			link.addEventListener('click', (event) => {
				event.preventDefault();
			});
		}
	}
	
	for (let link of footerLinks) {
		if (link.getAttribute('href') == location.pathname) {
			// Prevent reloading the page
			link.addEventListener('click', (event) => {
				event.preventDefault();
			});
		}
	}
});
