import React from 'react';
import { Link } from 'react-router-dom';

export const ContentMenu = () => {
  return (
    <ul className="navbar-nav mr-auto">
      <li className="nav-item">
        <Link className="nav-link js-scroll-trigger" to="#features">Features</Link>
      </li>
      <li className="nav-item">
        <Link className="nav-link js-scroll-trigger" to="#technology">Technology</Link>
      </li>
      <li className="nav-item">
        <Link className="nav-link js-scroll-trigger" to="#contact">Contact</Link>
      </li>
      <li className="nav-item">
        <Link className="nav-link js-scroll-trigger" to="#about">About</Link>
      </li>
    </ul>
  );
};
