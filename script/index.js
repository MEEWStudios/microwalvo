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
	
	let showcases = document.getElementsByClassName('showcase');
	for (let showcase of showcases) {
		let images = showcase.getElementsByTagName('img');
		
		for (let image of images) {
			image.addEventListener('click', (event) => {
				event.preventDefault();
				
				let overlay = document.createElement('div');
				overlay.className = 'theater';
				let screen = document.createElement('div');
				screen.className = 'screen';
				let largeImage = document.createElement('img');
				largeImage.src = image.src;
				let browser = document.createElement('div');
				browser.className = 'browser';
				screen.appendChild(largeImage);
				overlay.appendChild(screen);
				overlay.appendChild(browser);
				document.body.appendChild(overlay);
				
				let selectedThumbnail;
				
				// Populate image browser
				for (let thumbnailSource of images) {
					let thumbnail = document.createElement('img');
					thumbnail.src = thumbnailSource.src;
					if (thumbnailSource === image) {
						selectedThumbnail = thumbnail;
						thumbnail.classList.add('selected');
					}
					// LEAK
					thumbnail.addEventListener('click', (event) => {
						event.preventDefault();
						
						selectedThumbnail.classList.remove('selected');
						selectedThumbnail = thumbnail;
						thumbnail.classList.add('selected');
						largeImage.src = thumbnailSource.src;
					});
					
					browser.appendChild(thumbnail);
				}
				
				let close = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
				close.setAttribute('class', 'close');
				close.setAttribute('viewBox', '0 0 24 24');
				let path1 = document.createElementNS(close.namespaceURI, 'path');
				path1.setAttribute('d', 'M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z');
				path1.setAttribute('fill', 'white');
				path1.style.stroke = '#000';
				path1.style.strokeWidth = '0.5px';
				let path2 = document.createElementNS(close.namespaceURI, 'path');
				path2.setAttribute('d', 'M0 0h24v24H0z');
				path2.setAttribute('fill', 'none');
				close.appendChild(path1);
				close.appendChild(path2);
				overlay.appendChild(close);
				
				function closeTheater(event) {
					if (event.target === largeImage || isInHeirarchyOf(event.target, browser)) {
						return;
					}
					
					overlay.removeEventListener('click', closeTheater);
					overlay.addEventListener('transitionend', (event) => {
						// Enable scrollbar
						document.body.style.overflowY = '';
						document.body.style.marginRight = '';
						document.body.removeChild(overlay);
					}, {once: true});
					
					overlay.classList.remove('open');
				}
				
				requestAnimationFrame(() => {
					requestAnimationFrame(() => {
						if (window.innerHeight < document.body.clientHeight) {
							// Disable scrollbar
							document.body.style.overflowY = 'hidden';
							document.body.style.marginRight = `${getScrollbarWidth()}px`;
						}
						
						if (overlay.style.backdropFilter === undefined) {
							// Darken the background if the browser does not support backdrop-filter
							overlay.style.background = 'rgba(0, 0, 0, 0.7)';
						}
						
						overlay.classList.add('open');
						overlay.addEventListener('click', closeTheater);
						
						requestAnimationFrame(() => {
							requestAnimationFrame(() => {
								selectedThumbnail.scrollIntoView();
								overlay.scrollTop = 0;
							});
						});
					});
				});
			});
		}
	}
});
