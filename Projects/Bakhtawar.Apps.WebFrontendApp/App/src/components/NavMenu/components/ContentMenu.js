import React from 'react';
import { useHistory } from 'react-router-dom';
import { NavItem, NavLink } from 'reactstrap';
import Scrollspy from 'react-scrollspy';
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
      <Scrollspy className="navbar-nav mr-auto" items={content.map((item) => item.path)} currentClassName="active" offset={56}>
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
      </Scrollspy>
    </>
  );
};
