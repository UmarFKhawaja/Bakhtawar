import React from 'react';
import { useHistory } from 'react-router-dom';
import { NavItem, NavLink } from 'reactstrap';
import { useContent } from 'components/Content';

export const ContentMenu = () => {
  const history = useHistory();
  const content = useContent();
  
  const handleClick = (e, url) => {
    e.preventDefault();

    history.replace(url);
  };

  return (
    <>
      <ul className="navbar-nav mr-auto">
        {
          content.map((section) => {
            const { path, text } = section;

            return (
              <NavItem key={path}>
                <NavLink href={`#${path}`} to={`#${path}`} onClick={(e) => handleClick(e, `#${path}`)}>{text}</NavLink>
              </NavItem>
            );
          })
        }
      </ul>
    </>
  );
};
