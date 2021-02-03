import React, { createContext, useContext } from 'react';

const content = [
  {
    path: 'features',
    text: 'Features',
    component: () => (
      <>
        <h2>Feature list</h2>
        <p className="lead">
          Lorem ipsum dolor sit amet, consectetur adipisicing elit. Aut optio velit inventore,
          expedita quo laboriosam possimus ea consequatur vitae, doloribus consequuntur ex. Nemo assumenda
          laborum vel, labore ut velit dignissimos.
        </p>
      </>
    )
  },
  {
    path: 'technology',
    text: 'Technology',
    component: () => (
      <>
        <h2>Technology stack</h2>
        <p className="lead">
          Lorem ipsum dolor sit amet, consectetur adipisicing elit. Vero odio fugiat voluptatem
          dolor, provident officiis, id iusto! Obcaecati incidunt, qui nihil beatae magnam et repudiandae ipsa
          exercitationem, in, quo totam:
        </p>
        <ul>
          <li>Phasellus pharetra vel urna elementum sagittis. Proin faucibus nulla sed dapibus condimentum.</li>
          <li>Curabitur maximus, sapien rhoncus ultricies rutrum, sapien ante faucibus dui, ut venenatis augue quam id
            arcu. Nam tristique ullamcorper enim, a ullamcorper lectus eleifend eget. Morbi vel urna in turpis eleifend
            posuere et ac orci.
          </li>
          <li>Integer sed ultricies ipsum. Nulla facilisi. Vestibulum eu imperdiet augue. Phasellus at eleifend tellus.
          </li>
          <li>Pellentesque pellentesque magna eget quam finibus, ac malesuada ante ultrices. Proin at consectetur magna,
            ut sagittis ante.
          </li>
        </ul>
      </>
    )
  },
  {
    path: 'contact',
    text: 'Contact',
    component: () => (
      <>
        <h2>Contact us</h2>
        <p className="lead">
          Lorem ipsum dolor sit amet, consectetur adipisicing elit. Vero odio fugiat voluptatem
          dolor, provident officiis, id iusto! Obcaecati incidunt, qui nihil beatae magnam et repudiandae ipsa
          exercitationem, in, quo totam.
        </p>
      </>
    )
  },
  {
    path: 'about',
    text: 'About',
    component: () => (
      <>
        <h2>About us</h2>
        <p className="lead">
          Lorem ipsum dolor sit amet, consectetur adipisicing elit. Vero odio fugiat voluptatem
          dolor, provident officiis, id iusto! Obcaecati incidunt, qui nihil beatae magnam et repudiandae ipsa
          exercitationem, in, quo totam.
        </p>
      </>
    )
  }
];

const ContentContext = createContext();

export const ContentProvider = ({ children }) => (
  <ContentContext.Provider value={content}>
    {children}
  </ContentContext.Provider>
);

export function useContent() {
  const content = useContext(ContentContext);
  
  return content;
}
